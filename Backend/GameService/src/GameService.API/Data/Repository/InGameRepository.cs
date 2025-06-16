using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using GameService.API.Business.Models;
using StackExchange.Redis;
using System.Text.Json;
using NReJSON;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly IDatabase _redisDb;

        public InGameRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisDb = connectionMultiplexer.GetDatabase();
        }

        public async Task AddGame(GameModel gameModel)
        {
            var entity = GameMapper.ToGameEntity(gameModel);
            var json = JsonSerializer.Serialize(entity);

            var batch = _redisDb.CreateBatch();
            var tasks = new List<Task>
           {
               batch.StringSetAsync($"game:{entity.Id}", json)
           };

            if (!string.IsNullOrWhiteSpace(entity.WhiteToken))
                tasks.Add(batch.StringSetAsync($"player:{entity.WhiteToken}", entity.Id.ToString()));
            if (!string.IsNullOrWhiteSpace(entity.BlackToken))
                tasks.Add(batch.StringSetAsync($"player:{entity.BlackToken}", entity.Id.ToString()));

            batch.Execute(); 
            await Task.WhenAll(tasks);
        }

        public async Task<List<GameModel>> GetAllGames()
        {
            var server = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints()[0]);
            var keys = server.Keys(pattern: "game:*").ToList();
            var games = new List<GameModel>();

            var tasks = keys.Select(key => _redisDb.StringGetAsync(key)).ToArray();
            var results = await Task.WhenAll(tasks);

            foreach (var json in results)
            {
                if (!json.IsNull)
                {
                    var entity = JsonSerializer.Deserialize<GameEntity>(json!.ToString());
                    if (entity != null)
                        games.Add(GameMapper.ToGameModel(entity));
                }
            }
            return games;
        }


        public async Task<GameModel?> GetGameByGameId(Guid gameId)
        {
            var redisKey = $"game:{gameId}";
            var json = await _redisDb.StringGetAsync(redisKey);

            if (json.IsNull)
                return null;

            var jsonString = json.ToString();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var entity = jsonString.TrimStart().StartsWith("[")
                ? JsonSerializer.Deserialize<List<GameEntity>>(jsonString, options)?.FirstOrDefault()
                : JsonSerializer.Deserialize<GameEntity>(jsonString, options);

            return entity != null ? GameMapper.ToGameModel(entity) : null;
        }


        public async Task<Guid> GetGameByPlayerId(string playerToken)
        {
            if (string.IsNullOrWhiteSpace(playerToken))
                return Guid.Empty;

            var gameIdString = await _redisDb.StringGetAsync($"player:{playerToken}");
            if (gameIdString.IsNullOrEmpty)
                return Guid.Empty;

            if (Guid.TryParse(gameIdString, out var gameId))
                return gameId;

            return Guid.Empty;
        }

        public async Task UpdateGame(GameModel gameModel)
        {
            var entity = GameMapper.ToGameEntity(gameModel);
            var key = $"game:{entity.Id}";
            var exists = await _redisDb.KeyExistsAsync(key);
            if (!exists)
                throw new KeyNotFoundException($"Game with ID {gameModel.Id} does not exist.");
            var json = JsonSerializer.Serialize(entity);

            var batch = _redisDb.CreateBatch();
            var tasks = new List<Task>
            {
                batch.StringSetAsync(key, json)
            };
            if (!string.IsNullOrWhiteSpace(entity.WhiteToken))
                tasks.Add(batch.StringSetAsync($"player:{entity.WhiteToken}", entity.Id.ToString()));
            if (!string.IsNullOrWhiteSpace(entity.BlackToken))
                tasks.Add(batch.StringSetAsync($"player:{entity.BlackToken}", entity.Id.ToString()));

            batch.Execute();
            await Task.WhenAll(tasks);
        }
    }
}
