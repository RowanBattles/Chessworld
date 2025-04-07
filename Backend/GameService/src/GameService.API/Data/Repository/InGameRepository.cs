using GameService.API.Business.Interfaces;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using System.Collections.Concurrent;
using GameService.API.Business.Models;

namespace GameService.API.Data.Repository
{
    public class InGameRepository : IGameRepository
    {
        private readonly ConcurrentBag<GameEntity> _activeGames = [];

        public async Task AddGame(GameModel gameModel)
        {
            GameEntity gameEntity = GameMapper.ToGameEntity(gameModel);
            _activeGames.Add(gameEntity);
            await Task.CompletedTask;
        }

        public async Task<GameModel?> GetGameByGameId(Guid gameId)
        {
            var game = _activeGames.FirstOrDefault(g => g.Id == gameId);
            return await Task.FromResult(game != null ? GameMapper.ToGameModel(game) : null);
        }

        public async Task<Guid> GetGameByPlayerId(string playerToken)
        {
            var game = _activeGames.FirstOrDefault(g => g.WhiteToken == playerToken || g.BlackToken == playerToken);
            return await Task.FromResult(game?.Id ?? Guid.Empty);
        }

        public async Task UpdateGame(GameModel gameModel)
        {
            GameEntity? gameEntity = _activeGames.FirstOrDefault(g => g.Id == gameModel.Id);
            if (gameEntity != null)
            {
                _activeGames.TryTake(out gameEntity);
                gameEntity = GameMapper.ToGameEntity(gameModel);
                _activeGames.Add(gameEntity);
            }
            else
            {
                throw new KeyNotFoundException("Game not found");
            }
            await Task.CompletedTask;
        }
    }
}
