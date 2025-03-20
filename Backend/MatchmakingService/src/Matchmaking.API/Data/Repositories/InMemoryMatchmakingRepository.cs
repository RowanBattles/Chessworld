using Matchmaking.API.Data.Interfaces;
using System.Collections.Concurrent;

namespace GameService.API.Data.Repositories
{
    public class InMemoryMatchmakingRepository : IMatchmakingRepository
    {
        private static readonly ConcurrentQueue<string> _players = new();

        public void DequeuePlayer(string opponentToken)
        {
            var playersList = _players.ToList();
            _players.Clear();
            foreach (var player in playersList)
            {
                if (player != opponentToken)
                {
                    _players.Enqueue(player);
                }
            }
        }

        public void EnqueuePlayer(string playerToken)
        {
            _players.Enqueue(playerToken);
        }

        public string? GetFirstPlayerInQueue()
        {
            if (_players.IsEmpty)
            {
                return null;
            }

            _players.TryPeek(out string? firstPlayer);
            return firstPlayer!;
        }
    }
}
