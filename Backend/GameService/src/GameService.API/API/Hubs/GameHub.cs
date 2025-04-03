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
            if (httpContext == null || connectionId == null)
            {
                Context.Abort();
                return;
            }

            string path = httpContext.Request.Path.Value ?? "";
            string? gameId = httpContext.Request.Query["gameId"];
            string? token = httpContext.Request.Query["token"];

            if (string.IsNullOrEmpty(gameId))
            {
                Context.Abort();
                return;
            }

            try
            {
                (string status, string? validToken, string color) = await _gameService.GetGameByGameId(token, gameId);

                if (path.StartsWith("/play"))
                {
                    if (string.IsNullOrEmpty(token) || validToken != token)
                    {
                        Context.Abort();
                        return;
                    }

                    await _gameConnectionService.AddConnectionAsync(Guid.Parse(gameId), connectionId, color);
                }
                else if (path.StartsWith("/watch"))
                {
                    await _gameConnectionService.AddConnectionAsync(Guid.Parse(gameId), connectionId, null);
                }
                else
                {
                    Context.Abort();
                }
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
    }
}
