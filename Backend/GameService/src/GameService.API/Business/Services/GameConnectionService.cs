using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;

namespace GameService.API.Business.Services
{
    public class GameConnectionService : IGameConnectionService
    {
        private readonly IGameConnectionRepository _repository;
        private readonly ILogger<GameConnectionService> _logger;

        public GameConnectionService(IGameConnectionRepository repository, ILogger<GameConnectionService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<(bool, bool, int)> AddConnectionAsync(Guid gameId, string connectionId, string? color)
        {
            try
            {
                GameConnectionModel? gameConnection = await GetGameConnectionAsync(gameId);

                if (gameConnection != null)
                {
                    gameConnection.AddConnection(connectionId, color);
                    await _repository.UpdateGameConnectionAsync(gameConnection);
                    return gameConnection.StatusResponse();
                }

                var newConnection = new GameConnectionModel(gameId);
                newConnection.AddConnection(connectionId, color);
                await _repository.AddGameConnectionAsync(newConnection);
                return newConnection.StatusResponse();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding connection");
                throw;
            }
        }

        public async Task<string?> GetColorByConnectionId(Guid gameId, string connectionId)
        {
            GameConnectionModel? gameConnection = await GetGameConnectionAsync(gameId);

            if (gameConnection == null)
            {
                throw new Exception("Game connection not found");
            }

            string? color = gameConnection.GetColorByConnectionId(connectionId);
            return color;
        }

        public async Task<Guid> GetGameIdByConnectionId(string connectionId)
        {
            try
            {
                Guid gameId = await _repository.GetGameConnectionByConnectionIdAsync(connectionId);

                if (gameId == Guid.Empty)
                {
                    throw new Exception("Game connection not found");
                }

                return gameId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game ID by connection ID");
                throw;
            }
        }

        private async Task<GameConnectionModel?> GetGameConnectionAsync(Guid gameId)
        {
            try
            {
                return await _repository.GetGameConnectionAsync(gameId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game connection");
                throw;
            }
        }
    }
}
