using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;

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

        public async Task CreateGame(GameModel gameModel)
        {
            try
            {
                await _gameRepository.AddGame(gameModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding game to repository");
                throw new Exception("Game could not be created", ex);
            }
        }

        public async Task<Guid> GetStatusByPlayerId(string playerToken)
        {
            try
            {
                return await _gameRepository.GetGameByPlayerId(playerToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game for playerToken");
                throw new Exception("Game could not be retrieved", ex);
            }
        }

        public async Task<(string, string?, string)> GetGameByGameId(string? playerToken, string gameId)
        {
            try
            {
                var gameModel = await _gameRepository.GetGameByGameId(gameId) ?? throw new KeyNotFoundException("Game not found");

                string color = playerToken == gameModel.WhiteToken ? "white" :
                                playerToken == gameModel.BlackToken ? "black" :
                                "white";

                string? validToken = playerToken == gameModel.WhiteToken ? gameModel.WhiteToken :
                                    playerToken == gameModel.BlackToken ? gameModel.BlackToken :
                                    null;

                return (gameModel.Status.ToString(), validToken, color);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Game not found for gameId");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game for gameId");
                throw new InvalidOperationException("Game could not be retrieved", ex);
            }
        }
    }
}