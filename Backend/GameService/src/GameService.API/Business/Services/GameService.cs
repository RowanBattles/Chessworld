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

        public async Task<(string, string)> GetGameByGameId(string? playerToken, string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                throw new ArgumentException("Game ID cannot be null or empty", nameof(gameId));
            }

            try
            {
                var gameModel = await _gameRepository.GetGameByGameId(gameId);

                if (gameModel == null)
                {
                    throw new KeyNotFoundException("Game not found");
                }

                string role = playerToken switch
                {
                    _ when playerToken == gameModel.WhiteToken => "white",
                    _ when playerToken == gameModel.BlackToken => "black",
                    _ => "spectator"
                };

                return (role, gameModel.Status.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving game for gameId");
                throw new InvalidOperationException("Game could not be retrieved", ex);
            }
        }
    }
}