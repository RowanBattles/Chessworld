using System.Collections.Concurrent;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;
using GameService.API.Data.Entity;

namespace GameService.API.Data.Repository
{
    public class InGameConnectionRepository : IGameConnectionRepository
    {
        private readonly ConcurrentDictionary<Guid, GameConnectionEntity> _activeConnections = new();
        private readonly ILogger<InGameConnectionRepository> _logger;

        public InGameConnectionRepository(ILogger<InGameConnectionRepository> logger)
        {
            _logger = logger;
        }

        public async Task<GameConnectionModel?> GetGameConnectionAsync(Guid gameId)
        {
            if (_activeConnections.TryGetValue(gameId, out var gameConnection))
            {
                return await Task.FromResult(GameConnectionMapper.ToModel(gameConnection));
            }
            return await Task.FromResult<GameConnectionModel?>(null);
        }

        public async Task AddGameConnectionAsync(GameConnectionModel gameConnectionModel)
        {
            var entity = GameConnectionMapper.ToEntity(gameConnectionModel);
            if (!_activeConnections.TryAdd(entity.GameId, entity))
            {
                _logger.LogWarning("Game connection already exists for gameId: {GameId}", entity.GameId);
                throw new InvalidOperationException("Game connection already exists.");
            }
            await Task.CompletedTask;
        }

        public async Task UpdateGameConnectionAsync(GameConnectionModel gameConnectionModel)
        {
            var entity = GameConnectionMapper.ToEntity(gameConnectionModel);
            _activeConnections[entity.GameId] = entity;
            await Task.CompletedTask;
        }

        public async Task<Guid> GetGameConnectionByConnectionIdAsync(string connectionId)
        {
            foreach (var gameConnection in _activeConnections.Values)
            {
                if (gameConnection.ConnectionWhite == connectionId || gameConnection.ConnectionBlack == connectionId || gameConnection.ConnectionSpectators.Contains(connectionId))
                {
                    return await Task.FromResult(gameConnection.GameId);
                }
            }
            return await Task.FromResult(Guid.Empty);
        }
    }
}
