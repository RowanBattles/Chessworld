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

        public Task<Guid> GetGameByPlayerIdAsync(Guid playerId)
        {
            return Task.Run(() => {
                foreach (var game in _Activegames)
                {
                    if (game.White == playerId || game.Black == playerId)
                    {
                        return game.Id;
                    }
                }
                return Guid.Empty;
            });
        }
    }
}
