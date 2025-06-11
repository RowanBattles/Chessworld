using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;

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
                _logger.LogWarning("OnConnectedAsync aborted: Missing HttpContext or connection ID.");
                Context.Abort();
                return;
            }

            string path = httpContext.Request.Path.Value ?? "";
            string? gameId = httpContext.Request.Query["gameId"];
            string? token = httpContext.Request.Query["token"];

            if (string.IsNullOrEmpty(gameId) || !Guid.TryParse(gameId, out Guid gameGuid))
            {
                _logger.LogWarning("OnConnectedAsync aborted: Invalid or missing gameId.");
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
                        _logger.LogWarning("OnConnectedAsync aborted: Invalid or missing token for game {GameId}.", gameId);
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
                    _logger.LogWarning("OnConnectedAsync aborted: Invalid path {Path} for connection {ConnectionId}.", path, connectionId);
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

            if (string.IsNullOrEmpty(connectionId))
            {
                _logger.LogWarning("MakeMove aborted: Missing connection ID.");
                Context.Abort();
                return;
            }

            try
            {
                Guid gameId = await _gameConnectionService.GetGameIdByConnectionId(connectionId);
                string? color = await _gameConnectionService.GetColorByConnectionId(gameId, connectionId);

                if (color == null)
                {
                    _logger.LogWarning("MakeMove aborted: Connection {ConnectionId} has no assigned color.", connectionId);
                    await Clients.Caller.SendAsync("Error", "You are not assigned a color in this game.");
                    return;
                }

                string fen = await _gameService.MakeMove(gameId, color, uci);

                await Clients.Group(gameId.ToString()).SendAsync("ReceiveMove", fen);

                _logger.LogInformation("Move {Uci} made by connection {ConnectionId} in game {GameId}.", uci, connectionId, gameId);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Validation failed for UCI move {Uci}.", uci);
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "MakeMove failed: Game not found for connection {ConnectionId}.", connectionId);
                await Clients.Caller.SendAsync("Error", "Game not found.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "MakeMove failed: Invalid move {Uci} by connection {ConnectionId}.", uci, connectionId);
                await Clients.Caller.SendAsync("Error", "Invalid move.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during MakeMove for connection {ConnectionId}.", connectionId);
                await Clients.Caller.SendAsync("Error", "An unexpected error occurred.");
            }
        }
    }
}
