using System.Collections.Concurrent;

namespace GameService.API.src.Data.Repositories
{
    public class InGameMemoryRepository : IGameRepository
    {
        private static ConcurrentQueue<string> waitingPlayers = new();
        private static readonly ConcurrentDictionary<string, string> activeGames = new();

        public bool TryDequeueOpponent(out string? opponentId)
        {
            return waitingPlayers.TryDequeue(out opponentId);
        }

        public void EnqueuePlayer(string playerId)
        {
            waitingPlayers.Enqueue(playerId);
        }

        public void AddToActiveGames(string playerId, string opponentId)
        {
            activeGames[playerId] = opponentId;
            activeGames[opponentId] = playerId;
        }

        public bool PlayerInGame(string playerId)
        {
            return activeGames.ContainsKey(playerId);
        }

        public string? GetOpponent(string playerId)
        {
            activeGames.TryGetValue(playerId, out string? opponentId);
            return opponentId;
        }

        public void RemoveFromGame(string playerId, string opponentId)
        {
            activeGames.TryRemove(playerId, out _);
            activeGames.TryRemove(opponentId, out _);
        }

        public void RemoveFromWaitingQueue(string playerId)
        {
            waitingPlayers = new ConcurrentQueue<string>(waitingPlayers.Where(id => id != playerId));
        }
    }
}
