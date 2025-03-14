
namespace Matchmaking.API.Business.Infrastructure
{
    public interface IMatchmakingService
    {
        Task<(bool, string)> FindAndCreateGameAsync();
        string? GetMatchStatus(string playerId);
    }
}
