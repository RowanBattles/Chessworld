using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using System.Collections.Concurrent;
using GameService.API.Business.Models;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly ConcurrentBag<GameEntity> _games = new();

        public Task AddGameAsync(GameModel gameModel)
        {
            return Task.Run(() =>
            {
                GameEntity gameEntity = GameMapper.ToGameEntity(gameModel);
                _games.Add(gameEntity);
            });
        }
    }
}
