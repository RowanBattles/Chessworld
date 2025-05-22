using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameService
    {
        Task CreateGame(GameModel gameModel);
        Task<Guid> GetStatusByPlayerId(string playerToken);
        Task<(string status, string fen, string? token, string color)> GetGameByGameId(string? playerToken, Guid gameId);
        Task<string> MakeMove(Guid gameId, string? color, string uci);
        Task<List<GameModel>> GetAllGames();
    }
}