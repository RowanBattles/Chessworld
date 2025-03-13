using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using System.Collections.Concurrent;
using GameService.API.Business.Models;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly ConcurrentDictionary<Guid, GameEntity> _games = new();
        public bool AddGame(GameModel gameModel)
        {
            GameEntity gameEntity = GameMapper.ToGameEntity(gameModel);
            return _games.TryAdd(gameModel.Id, gameEntity);
        }
    }
}
