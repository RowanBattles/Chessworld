using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameService
    {
        Task CreateGameAsync(GameModel gameModel);
    }
}