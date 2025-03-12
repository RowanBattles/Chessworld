using Matchmaking.API.Data.Interfaces;

namespace GameService.API.Data.Repositories
{
    public class InMemoryMatchmakingRepository : IMatchmakingRepository
    {
        private static readonly Dictionary<Guid, string> _players = new();

        public void AddPlayer(Guid playerId, string connectionId)
        {
            _players.Add(playerId, connectionId);
        }
    }
}
