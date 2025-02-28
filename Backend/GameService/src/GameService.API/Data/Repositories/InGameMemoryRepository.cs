using GameService.API.Domain.DTOs;
using System.Collections.Concurrent;

namespace GameService.API.src.Data.Repositories
{
    public class InGameMemoryRepository : IGameRepository
    {
        private static ConcurrentQueue<Guid> waitingPlayers = new();
        private static readonly ConcurrentBag<Game> activeGames = [];
        private static readonly object queueLock = new object();

        public Guid? MatchWithFirstPersonInQueue()
        {
            if (waitingPlayers.TryDequeue(out var opponentId))
            {
                return opponentId;
            }
            return null;
        }

        public void EnqueuePlayer(Guid playerId)
        {
            waitingPlayers.Enqueue(playerId);
        }

        public void DequeuePlayer(Guid playerId)
        {
            lock (queueLock) // Make sure that no changes are being made to the queue while we are removing the player
            {
                var newQueue = new ConcurrentQueue<Guid>();

                while (waitingPlayers.TryDequeue(out var player))
                {
                    if (player != playerId)
                    {
                        newQueue.Enqueue(player);
                    }
                }

                Interlocked.Exchange(ref waitingPlayers, newQueue);
            }
        }

        public bool PlayerInQueue(Guid playerId)
        {
            return waitingPlayers.Contains(playerId);
        }

        public bool PlayerInGame(Guid playerId)
        {
            return activeGames.Any(g => g.Player1Id == playerId || g.Player2Id == playerId);
        }

        public void AddGame(Game game)
        {
            activeGames.Add(game);
        }

        public bool RemoveGame(Guid gameId)
        {
            var game = activeGames.FirstOrDefault(g => g.GameId == gameId);
            if (game != null)
            {
               return activeGames.TryTake(out game);
            }
            return false;
        }

        public Game? GetGameByPlayerId(Guid playerId)
        {
            return activeGames.FirstOrDefault(g => g.Player1Id == playerId || g.Player2Id == playerId);
        }
    }
}