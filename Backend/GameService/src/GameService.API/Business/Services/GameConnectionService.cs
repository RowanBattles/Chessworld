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

        public async Task AddConnectionAsync(Guid gameId, string connectionId, string? color)
        {
            try
            {
                GameConnectionModel? gameConnection = await _repository.GetGameConnectionAsync(gameId);

                if (gameConnection != null)
                {
                    gameConnection.AddConnection(connectionId, color);
                    await _repository.UpdateGameConnectionAsync(gameConnection);
                    return;
                }
                await _repository.AddGameConnectionAsync(new GameConnectionModel(gameId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding connection");
                throw;
            }
        }
    }
}
