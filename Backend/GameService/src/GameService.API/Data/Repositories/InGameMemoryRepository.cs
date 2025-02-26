using System.Collections.Concurrent;

namespace GameService.API.src.Data.Repositories
{
    public class InGameMemoryRepository : IGameRepository
    {
        private static ConcurrentQueue<Guid> waitingPlayers = new();
        private static readonly ConcurrentDictionary<Guid, Guid> activeGames = new();

        public bool TryDequeueOpponent(out Guid opponentId)
        {
            return waitingPlayers.TryDequeue(out opponentId);
        }

        public void EnqueuePlayer(Guid playerId)
        {
            waitingPlayers.Enqueue(playerId);
        }

        public void AddToActiveGames(Guid playerId, Guid opponentId)
        {
            activeGames[playerId] = opponentId;
            activeGames[opponentId] = playerId;
        }

        public bool PlayerInGame(Guid playerId)
        {
            return activeGames.ContainsKey(playerId);
        }

        public Guid? GetOpponent(Guid playerId)
        {
            activeGames.TryGetValue(playerId, out Guid opponentId);
            return opponentId;
        }

        public void RemoveFromGame(Guid playerId, Guid opponentId)
        {
            activeGames.TryRemove(playerId, out _);
            activeGames.TryRemove(opponentId, out _);
        }

        public void RemoveFromWaitingQueue(Guid playerId)
        {
            waitingPlayers = new ConcurrentQueue<Guid>(waitingPlayers.Where(id => id != playerId));
        }
    }
}
