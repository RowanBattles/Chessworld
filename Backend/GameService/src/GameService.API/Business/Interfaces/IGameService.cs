using GameService.API.API.Responses;
using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameService
    {
        Task<Guid> CreateGame(GameModel gameModel);
    }
}