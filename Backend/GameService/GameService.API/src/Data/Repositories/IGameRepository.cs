namespace GameService.API.src.Data.Repositories
{
    public interface IGameRepository
    {
        bool TryDequeueOpponent(out string? opponentId);
        void EnqueuePlayer(string playerId);
        void AddToActiveGames(string playerId, string opponentId);
        bool PlayerInGame(string playerId);
        string? GetOpponent(string playerId);
        void RemoveFromGame(string playerId, string opponentId);
        void RemoveFromWaitingQueue(string playerId);
    }
}
