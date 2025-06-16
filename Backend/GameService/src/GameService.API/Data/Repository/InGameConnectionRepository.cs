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
            var redisKey = $"gameconnection:{gameId}";
            var json = await _redisDb.StringGetAsync(redisKey);
            if (json.IsNullOrEmpty) return null;
            var entity = JsonSerializer.Deserialize<GameConnectionEntity>(json.ToString());
            return entity != null ? GameConnectionMapper.ToModel(entity) : null;
        }

        public async Task UpsertGameConnectionAsync(GameConnectionModel gameConnectionModel)
        {
            var entity = GameConnectionMapper.ToEntity(gameConnectionModel);
            var key = $"gameconnection:{entity.GameId}";
            var json = JsonSerializer.Serialize(entity);

            await _redisDb.StringSetAsync(key, json);

            if (!string.IsNullOrWhiteSpace(entity.ConnectionWhite))
                await _redisDb.StringSetAsync($"connection:{entity.ConnectionWhite}", entity.GameId.ToString());
            if (!string.IsNullOrWhiteSpace(entity.ConnectionBlack))
                await _redisDb.StringSetAsync($"connection:{entity.ConnectionBlack}", entity.GameId.ToString());
            if (entity.ConnectionSpectators != null)
            {
                foreach (var spectatorId in entity.ConnectionSpectators)
                {
                    if (!string.IsNullOrWhiteSpace(spectatorId))
                    {
                        await _redisDb.StringSetAsync($"connection:{spectatorId}", entity.GameId.ToString());
                    }
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

        public async Task AddConnectionAsync(string connectionId, string gameId, string? color)
        {
            var guid = Guid.Parse(gameId);
            var model = await GetGameConnectionAsync(guid) ?? new GameConnectionModel(guid);

            if (color == "white")
            {
                model.ConnectionWhite = connectionId;
            }
            else if (color == "black")
            {
                model.ConnectionBlack = connectionId;
            }
            else
            {
                model.ConnectionSpectators ??= new List<string>();
                if (!model.ConnectionSpectators.Contains(connectionId))
                    model.ConnectionSpectators.Add(connectionId);
            }

            await UpsertGameConnectionAsync(model);
        }

        public async Task RemoveConnectionAsync(string connectionId, string gameId)
        {
            var guid = Guid.Parse(gameId);
            var model = await GetGameConnectionAsync(guid);
            if (model == null) return;
            if (model.ConnectionWhite == connectionId)
                model.ConnectionWhite = null;
            else if (model.ConnectionBlack == connectionId)
                model.ConnectionBlack = null;
            else
                model.ConnectionSpectators?.Remove(connectionId);
            await UpsertGameConnectionAsync(model);
        }

        public async Task<string?> GetColor(Guid gameId, string connectionId)
        {
            var model = await GetGameConnectionAsync(gameId);
            if (model == null) return null;
            if (model.ConnectionWhite == connectionId)
                return "white";
            if (model.ConnectionBlack == connectionId)
                return "black";
            if (model.ConnectionSpectators != null && model.ConnectionSpectators.Contains(connectionId))
                return null;
            return null;
        }
    }
}
