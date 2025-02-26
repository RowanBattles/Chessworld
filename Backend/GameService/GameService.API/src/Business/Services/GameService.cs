using GameService.API.src.Data.Repositories;
using GameService.API.src.Domain.DTOs;

namespace GameService.API.src.Business.Services
{
    public class GameService(IGameRepository gameRepository) : IGameService
    {
        private readonly IGameRepository _gameRepository = gameRepository;

        public FindGameResponseDto MatchPlayer(string playerId)
        {
            if (_gameRepository.TryDequeueOpponent(out string? opponentId) && opponentId != null)
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

        public string? RemovePlayer(string playerId)
        {
            if (_gameRepository.PlayerInGame(playerId))
            {
                string? opponentId = _gameRepository.GetOpponent(playerId);
                if (opponentId != null)
                {
                    _gameRepository.RemoveFromGame(playerId, opponentId);
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
