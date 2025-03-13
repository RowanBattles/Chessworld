using Matchmaking.API.Data.Interfaces;
using System.Collections.Concurrent;

namespace GameService.API.Data.Repositories
{
    public class InMemoryMatchmakingRepository : IMatchmakingRepository
    {
        private static readonly ConcurrentQueue<Guid> _players = new();

        public void EnqueuePlayer(Guid playerId)
        {
            _players.Enqueue(playerId);
        }

        public Guid? GetFirstPlayerInQueue()
        {
            if (_players.IsEmpty)
            {
                return null;
            }

            _players.TryPeek(out Guid firstPlayer);
            return firstPlayer;
        }
    }
}
