using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GameService.API.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IGameService gameService, ILogger<GameHub> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        //private async Task JoinGame(string gameId)
        //{
        //    try
        //    {
        //        var playerToken = Context.GetHttpContext()?.Request.Cookies["playerToken"];
        //        (string role, string status) = await _gameService.GetGameByGameId(playerToken, gameId);

        //        // TODO: Add to group

        //        GameResponse response = new(role, status);
        //        await Clients.Caller.SendAsync("GameJoined", response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"An error occurred while joining the game with ID {gameId}.");
        //        await Clients.Caller.SendAsync("Error", "An error occurred while joining the game.");
        //    }
        //}
    }
}
