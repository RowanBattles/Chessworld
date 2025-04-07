using ChessDotNet;
using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Services;
using GameService.API.Contract.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameService.API.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameConnectionService _gameConnectionService;
        private readonly IGameService _gameService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IGameConnectionService gameConnectionService, IGameService gameService, ILogger<GameHub> logger)
        {
            _gameConnectionService = gameConnectionService;
            _gameService = gameService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            string connectionId = Context.ConnectionId;
            if (httpContext == null || string.IsNullOrEmpty(connectionId))
            {
                Context.Abort();
                return;
            }

            string path = httpContext.Request.Path.Value ?? "";
            string? gameId = httpContext.Request.Query["gameId"];
            string? token = httpContext.Request.Query["token"];

            if (string.IsNullOrEmpty(gameId) || !Guid.TryParse(gameId, out Guid gameGuid))
            {
                Context.Abort();
                return;
            }

            try
            {
                (string status, string fen, string? validToken, string color) = await _gameService.GetGameByGameId(token, gameGuid);

                bool white = false;
                bool black = false; 
                int watchers = 0;

                if (path.StartsWith("/play"))
                {
                    if (string.IsNullOrEmpty(token) || validToken != token)
                    {
                        Context.Abort();
                        return;
                    }

                    (white, black, watchers) = await _gameConnectionService.AddConnectionAsync(Guid.Parse(gameId), connectionId, color);
                }
                else if (path.StartsWith("/watch"))
                {
                    (white, black, watchers) = await _gameConnectionService.AddConnectionAsync(Guid.Parse(gameId), connectionId, null);
                }
                else
                {
                    Context.Abort();
                    return;
                }

                await Groups.AddToGroupAsync(connectionId, gameId);

                ConnectionStatusResponse connectionStatusResponse = new(white, black, watchers);
                await Clients.Group(gameId).SendAsync("ConnectionStatus", connectionStatusResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during OnConnectedAsync");
                Context.Abort();
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Connection {Context.ConnectionId} disconnected.");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task MakeMove(string uci)
        {
            string connectionId = Context.ConnectionId;

            if (string.IsNullOrEmpty(connectionId) || string.IsNullOrEmpty(uci))
            {
                Context.Abort();
                return;
            }

            try
            {
                Guid gameId = await _gameConnectionService.GetGameIdByConnectionId(connectionId);
                string? color = await _gameConnectionService.GetColorByConnectionId(gameId, connectionId);
                await _gameService.MakeMove(gameId, color, uci);
            }
            catch
            {

            }
        }
    }
}
