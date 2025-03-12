
namespace Matchmaking.API.Data.Interfaces
{
    public interface IMatchmakingRepository
    {
        void AddPlayer(Guid playerId, string connectionId);
    }
}
