
namespace Matchmaking.API.Data.Interfaces
{
    public interface IMatchmakingRepository
    {
        void EnqueuePlayer(Guid playerId);
        Guid? GetFirstPlayerInQueue();
    }
}
