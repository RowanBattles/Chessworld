using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameRepository
    {
        Task AddGame(GameModel gameModel);
        Task<GameModel?> GetGameByGameId(string gameId);
        Task<Guid> GetGameByPlayerId(string playerToken);
    }
}
