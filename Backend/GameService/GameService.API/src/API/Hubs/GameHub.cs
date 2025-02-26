using Microsoft.AspNetCore.SignalR;
using GameService.API.src.Business.Services;
using GameService.API.src.Domain.DTOs;

namespace GameService.API.src.API.Hubs
{
    public class GameHub(IGameService gameService) : Hub
    {
        private readonly IGameService _gameService = gameService;

        public async Task onConnect()
        {
            await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        }

        public async Task FindGame(FindGameRequestDto request)
        {
            var response = _gameService.MatchPlayer(request.PlayerId);
            if (response.OpponentId != null)
            {
                await Clients.Client(request.PlayerId).SendAsync("GameFound", response);
                await Clients.Client(response.OpponentId).SendAsync("GameFound", response);
            }
            else
            {
                await Clients.Caller.SendAsync("WaitingForOpponent", response);
            }
        }

        public async Task LeaveGame(FindGameRequestDto request)
        {
            var opponentId = _gameService.RemovePlayer(request.PlayerId);
            if (opponentId != null)
            {
                await Clients.Client(opponentId).SendAsync("OpponentLeft");
            }
        }
    }
}
