using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using System.Collections.Concurrent;
using GameService.API.Business.Models;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly ConcurrentBag<GameEntity> _Activegames = new();

        public Task AddGameAsync(GameModel gameModel)
        {
            return Task.Run(() =>
            {
                GameEntity gameEntity = GameMapper.ToGameEntity(gameModel);
                _Activegames.Add(gameEntity);
            });
        }

        public Task<Guid> GetGameByPlayerIdAsync(string playerToken)
        {
            return Task.Run(() =>
            {
                foreach (var game in _Activegames)
                {
                    if (game.WhiteToken == playerToken || game.BlackToken == playerToken)
                    {
                        return game.Id;
                    }
                }
                return Guid.Empty;
            });
        }
    }
}
