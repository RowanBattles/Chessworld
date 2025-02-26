using GameService.API.src.Business.Services;
using GameService.API.src.Data.Repositories;
using GameService.API.src.Domain.DTOs;

namespace GameService.API.Business.Services
{
    public class GameService(IGameRepository gameRepository) : IGameService
    {
        private readonly IGameRepository _gameRepository = gameRepository;

        public FindGameResponseDto MatchPlayer(Guid playerId)
        {
            if (_gameRepository.TryDequeueOpponent(out Guid opponentId))
            {
                _gameRepository.AddToActiveGames(playerId, opponentId);
                return new FindGameResponseDto(opponentId, false);
            }
            else
            {
                _gameRepository.EnqueuePlayer(playerId);
                return new FindGameResponseDto(playerId, true);
            }
        }

        public Guid? RemovePlayer(Guid playerId)
        {
            if (_gameRepository.PlayerInGame(playerId))
            {
                Guid? opponentId = _gameRepository.GetOpponent(playerId);
                if (opponentId != null)
                {
                    _gameRepository.RemoveFromGame(playerId, opponentId.Value);
                    return opponentId;
                }
                else
                {
                    //TODO: throw exception and log error
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