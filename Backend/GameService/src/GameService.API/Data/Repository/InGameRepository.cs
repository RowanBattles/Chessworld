using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using GameService.API.Business.Models;
using StackExchange.Redis;
using System.Text.Json;
using NReJSON;

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
            await _redisDb.ExecuteAsync("JSON.SET", $"game:{entity.Id}", "$", json);
        }

        public async Task<List<GameModel>> GetAllGames()
        {
            var server = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints()[0]);
            var keys = server.Keys(pattern: "game:*");
            var games = new List<GameModel>();

            foreach (var key in keys)
            {
                var json = await _redisDb.JsonGetAsync(key);
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
            var json = await _redisDb.JsonGetAsync($"game:{gameId}");
            if (json.IsNull) return null;
            var entity = JsonSerializer.Deserialize<GameEntity>(json!.ToString());
            return GameMapper.ToGameModel(entity!);
        }

        public async Task<Guid> GetGameByPlayerId(string playerToken)
        {
            if (string.IsNullOrWhiteSpace(playerToken))
                return Guid.Empty;

            var server = _redisDb.Multiplexer.GetServer(_redisDb.Multiplexer.GetEndPoints()[0]);

            var gameKeys = server.Keys(pattern: "game:*");

            foreach (var key in gameKeys)
            {
                var json = await _redisDb.ExecuteAsync("JSON.GET", key);
                if (json.IsNull) continue;

                var game = JsonSerializer.Deserialize<GameEntity>(json.ToString());

                if (game is null) continue;

                if (game.WhiteToken == playerToken || game.BlackToken == playerToken)
                    return game.Id;
            }

            return Guid.Empty;
        }

        public async Task<bool> UpdateGame(GameModel gameModel)
        {
            var entity = GameMapper.ToGameEntity(gameModel);
            var exists = await _redisDb.KeyExistsAsync($"game:{entity.Id}");
            if (!exists)
                throw new KeyNotFoundException($"Game with ID {gameModel.Id} does not exist.");
            var json = JsonSerializer.Serialize(entity);
            await _redisDb.JsonSetAsync($"game:{entity.Id}", "$", json);
            return true;
        }
    }
}
