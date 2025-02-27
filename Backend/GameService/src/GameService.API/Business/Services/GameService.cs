using GameService.API.src.Business.Services;
using GameService.API.src.Data.Repositories;
using GameService.API.src.Domain.DTOs;

namespace GameService.API.Business.Services
{
    public class GameService(IGameRepository gameRepository) : IGameService
    {
        private readonly IGameRepository _gameRepository = gameRepository;

        public Guid? DisconnectPlayer(Guid? playerId)
        {
            if (playerId != null)
            {
                return RemovePlayer((Guid)playerId);
            }
            return null;
        }

        public FindGameResponseDto? MatchPlayer(Guid playerId)
        {
            if (_gameRepository.PlayerInQueue(playerId) || _gameRepository.PlayerInGame(playerId))
            {
                return new FindGameResponseDto(playerId);
            }
            else if (_gameRepository.TryDequeueOpponent(out Guid opponentId))
            {
                _gameRepository.AddToActiveGames(playerId, opponentId);
                return new FindGameResponseDto(opponentId);
            }
            else
            {
                _gameRepository.EnqueuePlayer(playerId);
                return null;
            }
        }

        public Guid? RemovePlayer(Guid playerId)
        {
            if (_gameRepository.PlayerInGame(playerId) || _gameRepository.PlayerInQueue(playerId))
            {
                Guid? opponentId = _gameRepository.GetOpponent(playerId);
                if (opponentId != null)
                {
                    _gameRepository.RemoveFromGame(playerId, opponentId.Value);
                    return opponentId;
                }
                else
                {
                    // TODO: Exception handling
                    return null;
                }
            }
            else
            {
                _gameRepository.RemoveFromWaitingQueue(playerId);
                return null;
            }
        }
    }
}