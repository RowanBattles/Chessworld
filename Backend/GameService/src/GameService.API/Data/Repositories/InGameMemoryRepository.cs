using System.Collections.Concurrent;

namespace GameService.API.src.Data.Repositories
{
    public class InGameMemoryRepository : IGameRepository
    {
        private static ConcurrentQueue<Guid> waitingPlayers = new();
        private static readonly ConcurrentDictionary<Guid, Guid> playerToOpponent = new();
        private static readonly ConcurrentDictionary<Guid, Guid> opponentToPlayer = new();

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
            playerToOpponent[playerId] = opponentId;
            opponentToPlayer[opponentId] = playerId;
        }

        public bool PlayerInQueue(Guid playerId)
        {
            return waitingPlayers.Contains(playerId);
        }

        public bool PlayerInGame(Guid playerId)
        {
            return playerToOpponent.ContainsKey(playerId) || opponentToPlayer.ContainsKey(playerId);
        }

        public Guid? GetOpponent(Guid playerId)
        {
            if (playerToOpponent.TryGetValue(playerId, out Guid opponentId))
            {
                return opponentId;
            }
            if (opponentToPlayer.TryGetValue(playerId, out Guid opponentIdFromOpponent))
            {
                return opponentIdFromOpponent;
            }
            return null;
        }

        public void RemoveFromGame(Guid playerId, Guid opponentId)
        {
            // TODO: Not working
            playerToOpponent.TryRemove(playerId, out _);
            opponentToPlayer.TryRemove(opponentId, out _);
        }

        public void RemoveFromWaitingQueue(Guid playerId)
        {
            waitingPlayers = new ConcurrentQueue<Guid>(waitingPlayers.Where(id => id != playerId));
        }
    }
}