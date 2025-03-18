
namespace Matchmaking.API.Business.Infrastructure
{
    public interface IMatchmakingService
    {
        Task<(bool, string)> FindAndCreateGameAsync();
        Task<(bool, string?, string)> GetMatchStatus(string playerId);
    }
}
