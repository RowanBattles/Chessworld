using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameRepository
    {
        bool AddGame(GameModel gameModel);
    }
}
