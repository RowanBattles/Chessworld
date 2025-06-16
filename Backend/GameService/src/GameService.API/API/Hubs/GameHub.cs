using GameService.API.Business.Interfaces;
using GameService.API.Data.Repository;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;

namespace GameService.API.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly InGameConnectionRepository _connectionRepo;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<GameHub> _logger;

        public GameHub(
            IGameService gameService,
            InGameConnectionRepository connectionRepo,
            IConnectionMultiplexer redis,
            ILogger<GameHub> logger)
        {
            _gameService = gameService;
            _connectionRepo = connectionRepo;
            _redis = redis;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                if (httpContext == null)
                {
                    _logger.LogError("HttpContext is null for ConnectionId: {ConnectionId}", Context.ConnectionId);
                    await Clients.Caller.SendAsync("Error", "Unable to retrieve HttpContext.");
                    return;
                }

                var gameId = httpContext.Request.Query["gameId"].ToString();
                if (string.IsNullOrEmpty(gameId))
                {
                    _logger.LogError("GameId is null or empty for ConnectionId: {ConnectionId}", Context.ConnectionId);
                    await Clients.Caller.SendAsync("Error", "Game ID is required.");
                    return;
                }

                var token = httpContext.Request.Query["token"].ToString();

                string? color = null;
                if (!string.IsNullOrEmpty(token) && Guid.TryParse(gameId, out var gameGuid))
                {
                    var gameInfo = await _gameService.GetGameByGameId(token, gameGuid);
                    color = gameInfo.color;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

                await _connectionRepo.AddConnectionAsync(Context.ConnectionId, gameId, color);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync for ConnectionId: {ConnectionId}", Context.ConnectionId);
                await Clients.Caller.SendAsync("Error", "An error occurred while connecting.");
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var gameId = httpContext?.Request.Query["gameId"].ToString();

                if (!string.IsNullOrEmpty(gameId))
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
                    await _connectionRepo.RemoveConnectionAsync(Context.ConnectionId, gameId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync for ConnectionId: {ConnectionId}", Context.ConnectionId);
            }
        }

        public async Task MakeMove(string gameId, string? uci)
        {
            try
            {
                if (string.IsNullOrEmpty(uci))
                {
                    await Clients.Caller.SendAsync("Error", "Move (UCI) cannot be null or empty.");
                    return;
                }

                if (!Guid.TryParse(gameId, out var gameIdGuid))
                {
                    await Clients.Caller.SendAsync("Error", "Invalid game ID format.");
                    return;
                }

                var color = await _connectionRepo.GetColor(gameIdGuid, Context.ConnectionId);

                if (color == null)
                {
                    await Clients.Caller.SendAsync("Error", "Spectators cannot make moves.");
                    return;
                }

                var fen = await _gameService.MakeMoveAsync(gameIdGuid, color, uci);

                var subscriber = _redis.GetSubscriber();
                await subscriber.PublishAsync(RedisChannel.Literal($"game:{gameId}"), fen);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MakeMove for ConnectionId: {ConnectionId}, GameId: {GameId}", Context.ConnectionId, gameId);
                await Clients.Caller.SendAsync("Error", "An error occurred while making the move.");
            }
        }
    }
}