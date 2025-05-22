
namespace Matchmaking.API.Data.Interfaces
{
    public interface IMatchmakingRepository
    {
        void DequeuePlayer(string opponentToken);
        void EnqueuePlayer(string playerToken);
        string? GetFirstPlayerInQueue();
        string? TryDequeuePlayer();
    }
}
