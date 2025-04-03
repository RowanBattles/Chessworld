using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using System.Collections.Concurrent;
using GameService.API.Business.Models;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly ConcurrentBag<GameEntity> _Activegames = [];

        public async Task AddGame(GameModel gameModel)
        {
            GameEntity gameEntity = GameMapper.ToGameEntity(gameModel);
            _Activegames.Add(gameEntity);
            await Task.CompletedTask;
        }

        public async Task<GameModel?> GetGameByGameId(string gameId)
        {
            var game = _Activegames.FirstOrDefault(g => g.Id.ToString() == gameId);
            return await Task.FromResult(game != null ? GameMapper.ToGameModel(game) : null);
        }

        public async Task<Guid> GetGameByPlayerId(string playerToken)
        {
            var game = _Activegames.FirstOrDefault(g => g.WhiteToken == playerToken || g.BlackToken == playerToken);
            return await Task.FromResult(game?.Id ?? Guid.Empty);
        }
    }
}
