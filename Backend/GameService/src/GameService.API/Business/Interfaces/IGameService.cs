using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameService
    {
        Task CreateGame(GameModel gameModel);
        Task<Guid> GetStatusByPlayerId(string playerToken);
        Task<(string, string)> GetGameByGameId(string? playerToken, string gameId);
    }
}