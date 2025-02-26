using Microsoft.AspNetCore.SignalR;
using GameService.API.src.Business.Services;
using GameService.API.src.Domain.DTOs;

namespace GameService.API.API.Hubs
{
    public class GameHub(IGameService gameService) : Hub
    {
        private readonly IGameService _gameService = gameService;

        public override Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;
            return base.OnConnectedAsync();
        }

        public async Task FindGame(FindGameRequestDto request)
        {
            var response = _gameService.MatchPlayer(request.PlayerId);
            if (!response.IsWaiting)
            {
                await Clients.Client(request.PlayerId.ToString()).SendAsync("GameFound", response);
                await Clients.Client(response.OpponentId?.ToString() ?? string.Empty).SendAsync("GameFound", response);
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
                await Clients.Client(opponentId.ToString() ?? string.Empty).SendAsync("OpponentLeft");
            }
        }
    }
}
