using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;

namespace GameService.API.Business.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameService> _logger;

        public GameService(IGameRepository gameRepository, ILogger<GameService> logger)
        {
            _gameRepository = gameRepository;
            _logger = logger;
        }

        public async Task CreateGameAsync(GameModel gameModel)
        {
            try
            {
                await _gameRepository.AddGameAsync(gameModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding game to repository");
                throw new Exception("Game could not be created", ex);
            }
        }

        public async Task<Guid> GetGameByPlayerIdAsync(string playerToken)
        {
            try
            {
                return await _gameRepository.GetGameByPlayerIdAsync(playerToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game for playerToken");
                throw new Exception("Game could not be retrieved", ex);
            }
        }
    }
}