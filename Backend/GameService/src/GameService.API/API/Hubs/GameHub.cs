using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GameService.API.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync(); 
        }

        public async Task GetGame(string gameId)
        {
            var playerToken = Context.GetHttpContext()?.Request.Cookies["playerToken"];
            var (role, status) = await _gameService.GetGameByGameId(playerToken, gameId);
            var response = new GameResponse(role, status);
            await Clients.Caller.SendAsync("GameStatus", response);
        }
    }
}
