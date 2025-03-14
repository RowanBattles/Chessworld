
namespace Matchmaking.API.Data.Interfaces
{
    public interface IMatchmakingRepository
    {
        void DequeuePlayer(Guid opponentId);
        void EnqueuePlayer(Guid playerId);
        Guid? GetFirstPlayerInQueue();
    }
}
