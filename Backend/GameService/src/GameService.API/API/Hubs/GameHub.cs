using Microsoft.AspNetCore.SignalR;
using GameService.API.src.Business.Services;
using GameService.API.src.Domain.DTOs;
using System.Numerics;
using GameService.API.Business.Services;

namespace GameService.API.API.Hubs
{
    public class GameHub : Hub
    {
        private readonly IGameService _gameService;
        private readonly IPlayerService _playerService;

        public GameHub(IGameService gameService, IPlayerService playerService, IUserIdProvider userIdProvider)
        {
            _gameService = gameService;
            _playerService = playerService;
        }


        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            var playerId = _playerService.AddConnectionIdToPlayer(connectionId);

            await Clients.Caller.SendAsync("ReceivePlayerId", playerId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionIdPlayer = Context.ConnectionId;
            var playerId = _playerService.GetPlayerId(connectionIdPlayer);
            var opponentId = _gameService.DisconnectPlayer(playerId);

            if (opponentId != null)
            {
                var connectionIdOpponent = _playerService.GetConnectionId((Guid)opponentId);
                if (!string.IsNullOrEmpty(connectionIdOpponent))
                {
                    await Clients.Client(connectionIdOpponent).SendAsync("GameLeft", "Game ended");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task FindGame(FindGameRequestDto request)
        {
            var response = _gameService.MatchPlayer(request.PlayerId);
            if (response != null)
            {
                if (response.OpponentId != request.PlayerId)
                {
                    var connectionId = _playerService.GetConnectionId(response.OpponentId);
                    if (!string.IsNullOrEmpty(connectionId))
                    {
                        await Clients.Caller.SendAsync("GameFound", response.OpponentId);
                        await Clients.Client(connectionId).SendAsync("GameFound", request.PlayerId);
                    }
                    else
                    {
                        await Clients.Caller.SendAsync("WaitingForOpponent", response);
                    }
                }
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
                var connectionId = _playerService.GetConnectionId((Guid)opponentId);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await Clients.Caller.SendAsync("GameLeft", "Game ended");
                    await Clients.Client(connectionId).SendAsync("GameLeft", "Game ended");
                }
            }
            else
            {
                await Clients.Caller.SendAsync("GameLeft", "Queue left");
            }
        }
    }
}
