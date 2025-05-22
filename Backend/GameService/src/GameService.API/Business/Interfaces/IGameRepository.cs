using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameRepository
    {
        Task AddGame(GameModel gameModel);
        Task<List<GameModel>> GetAllGames();
        Task<GameModel?> GetGameByGameId(Guid gameId);
        Task<Guid> GetGameByPlayerId(string playerToken);
        Task<bool> UpdateGame(GameModel gameModel);
    }
}
