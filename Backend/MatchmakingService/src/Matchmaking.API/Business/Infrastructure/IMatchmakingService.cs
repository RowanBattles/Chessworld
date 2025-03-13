namespace Matchmaking.API.Business.Infrastructure
{
    public interface IMatchmakingService
    {
        Task<(bool matchFound, string? gameUrl)> FindAndCreateGameA();
    }
}
