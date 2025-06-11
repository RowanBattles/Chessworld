using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;
using GameService.API.Data.Entity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using NReJSON;

namespace GameService.API.Data.Repository
{
    public class InGameConnectionRepository : IGameConnectionRepository
    {
        private readonly IDatabase _redisDb;

        public InGameConnectionRepository(IConnectionMultiplexer connectionMultiplexer, ILogger<InGameConnectionRepository> logger)
        {
            _redisDb = connectionMultiplexer.GetDatabase();
        }

        public async Task<GameConnectionModel?> GetGameConnectionAsync(Guid gameId)
        {
            var redisResult = await _redisDb.ExecuteAsync("JSON.GET", $"gameconnection:{gameId}");
            if (redisResult.IsNull) return null;
            var entity = JsonSerializer.Deserialize<GameConnectionEntity>(redisResult.ToString());
            return entity != null ? GameConnectionMapper.ToModel(entity) : null;
        }

        public async Task UpsertGameConnectionAsync(GameConnectionModel gameConnectionModel)
        {
            var entity = GameConnectionMapper.ToEntity(gameConnectionModel);
            var key = $"gameconnection:{entity.GameId}";
            var json = JsonSerializer.Serialize(entity);

            // Store the game connection
            await _redisDb.ExecuteAsync("JSON.SET", key, "$", json);

            // Index each connectionId to the gameId
            if (!string.IsNullOrWhiteSpace(entity.ConnectionWhite))
                await _redisDb.StringSetAsync($"connection:{entity.ConnectionWhite}", entity.GameId.ToString());
            if (!string.IsNullOrWhiteSpace(entity.ConnectionBlack))
                await _redisDb.StringSetAsync($"connection:{entity.ConnectionBlack}", entity.GameId.ToString());
            if (entity.ConnectionSpectators != null)
            {
                foreach (var spectatorId in entity.ConnectionSpectators)
                {
                    if (!string.IsNullOrWhiteSpace(spectatorId))
                        await _redisDb.StringSetAsync($"connection:{spectatorId}", entity.GameId.ToString());
                }
            }
        }


        public async Task<Guid> GetGameConnectionByConnectionIdAsync(string connectionId)
        {
            var gameIdString = await _redisDb.StringGetAsync($"connection:{connectionId}");
            if (gameIdString.IsNullOrEmpty)
                return Guid.Empty;

            if (Guid.TryParse(gameIdString, out var gameId))
                return gameId;

            return Guid.Empty;
        }

    }
}
