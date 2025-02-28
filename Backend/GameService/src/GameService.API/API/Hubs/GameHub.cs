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

        public GameHub(IGameService gameService, IPlayerService playerService)
        {
            _gameService = gameService;
            _playerService = playerService;
        }

        public override async Task OnConnectedAsync()
        {
            string connectionId = Context.ConnectionId;

            var callerId = _playerService.AddConnectionId(connectionId);

            await Clients.Caller.SendAsync("ReceivePlayerId", callerId);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string connectionIdPlayer = Context.ConnectionId;

            var callerId = _playerService.RemoveConnectionId(connectionIdPlayer);

            if (callerId != null)
            {
                await HandlePlayerDisconnection((Guid)callerId);
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
            var playerId = _gameService.RemovePlayer(request.PlayerId);
            
            if (playerId != null)
            {
                if (playerId == request.PlayerId)
                {
                    await Clients.Caller.SendAsync("GameLeft", "Queue left");
                }
                else 
                {
                    var connectionId = _playerService.GetConnectionId((Guid)playerId);
                    if (!string.IsNullOrEmpty(connectionId))
                    {
                        await Clients.Caller.SendAsync("GameLeft", "You have left the game");
                        await Clients.Client(connectionId).SendAsync("GameLeft", "Opponent has left the game");
                    }
                    else
                    {
                        // Log exception error
                        await Clients.Caller.SendAsync("GameLeft", "Opponent not found");
                    }
                }
            }
            else
            {
                await Clients.Caller.SendAsync("GameLeft", "Not in game");
            }
        }

        private async Task HandlePlayerDisconnection(Guid callerId)
        {
            var playerId = _gameService.RemovePlayer(callerId);

            if ((playerId != null) && (playerId != callerId))
            {
                var connectionId = _playerService.GetConnectionId((Guid)playerId);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await Clients.Client(connectionId).SendAsync("GameLeft", "Your opponent has disconnected");
                }
            }
        }
    }
}
