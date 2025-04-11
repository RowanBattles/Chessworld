using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using System.Collections.Concurrent;
using GameService.API.Business.Models;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly ConcurrentDictionary<Guid, GameEntity> _gamesById = new();
        private readonly ConcurrentDictionary<string, Guid> _playerTokenToGameId = new();

        public async Task AddGame(GameModel gameModel)
        {
            var entity = GameMapper.ToGameEntity(gameModel);
            _gamesById[entity.Id] = entity;
            _playerTokenToGameId[entity.WhiteToken] = entity.Id;
            _playerTokenToGameId[entity.BlackToken] = entity.Id;
            await Task.CompletedTask;
        }

        public async Task<GameModel?> GetGameByGameId(Guid gameId)
        {
            return await Task.FromResult(
                _gamesById.TryGetValue(gameId, out var entity)
                    ? GameMapper.ToGameModel(entity)
                    : null
            );
        }

        public async Task<Guid> GetGameByPlayerId(string playerToken)
        {
            return await Task.FromResult(
                _playerTokenToGameId.TryGetValue(playerToken, out var gameId)
                    ? gameId
                    : Guid.Empty
            );
        }

        public async Task<bool> UpdateGame(GameModel gameModel)
        {
            var entity = GameMapper.ToGameEntity(gameModel);
            _gamesById[entity.Id] = entity;
            return await Task.FromResult(true);
        }
    }
}
