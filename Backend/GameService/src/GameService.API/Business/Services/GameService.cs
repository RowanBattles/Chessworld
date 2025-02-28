using GameService.API.Domain.DTOs;
using GameService.API.src.Business.Services;
using GameService.API.src.Data.Repositories;
using GameService.API.src.Domain.DTOs;

namespace GameService.API.Business.Services
{
    public class GameService(IGameRepository gameRepository) : IGameService
    {
        private readonly IGameRepository _gameRepository = gameRepository;

        private bool PlayerInQueue(Guid playerId)
        {
            return _gameRepository.PlayerInQueue(playerId);
        }

        private bool PlayerInGame(Guid playerId)
        {
            return _gameRepository.PlayerInGame(playerId);
        }

        public FindGameResponseDto? MatchPlayer(Guid playerId)
        {
            bool playerInQueue = PlayerInQueue(playerId);
            bool playerInGame = PlayerInGame(playerId);

            if (!playerInQueue && !playerInGame)
            {
                var opponentId = _gameRepository.MatchWithFirstPersonInQueue();
                if (opponentId != null)
                {
                    // Create new game
                    Game game = new(playerId, (Guid)opponentId);
                    _gameRepository.AddGame(game);
                    return new FindGameResponseDto((Guid)opponentId);
                }
                else
                {
                    // Queue player
                    _gameRepository.EnqueuePlayer(playerId);
                    return null;
                }
            }
            return null;
        }

        public Guid? RemovePlayer(Guid playerId)
        {
            bool playerInQueue = PlayerInQueue(playerId);
            bool playerInGame = PlayerInGame(playerId);

            if (playerInQueue)
            {
                // Remove player from queue
                _gameRepository.DequeuePlayer(playerId);
                return playerId;
            }
            if (playerInGame)
            {
                // Remove game
                Game? game = _gameRepository.GetGameByPlayerId(playerId);
                if (game != null)
                {
                    _gameRepository.RemoveGame(game.GameId);
                    var opponentId = game.Player1Id == playerId ? game.Player2Id : game.Player1Id;
                    return opponentId;
                }
            }
            return null;
        }
    }
}