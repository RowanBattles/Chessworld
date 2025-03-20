using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameRepository
    {
        Task AddGameAsync(GameModel gameModel);
        Task<Guid> GetGameByPlayerIdAsync(string playerToken);
    }
}
