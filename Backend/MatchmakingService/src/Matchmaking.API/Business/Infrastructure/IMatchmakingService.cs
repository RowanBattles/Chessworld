
namespace Matchmaking.API.Business.Infrastructure
{
    public interface IMatchmakingService
    {
        Task<(bool, string, string?)> FindAndCreateGameAsync();
        Task<(bool, string?, string)> GetMatchStatus(string playerToken);
    }
}
