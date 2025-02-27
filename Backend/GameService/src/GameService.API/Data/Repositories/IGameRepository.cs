namespace GameService.API.src.Data.Repositories
{
    public interface IGameRepository
    {
        bool TryDequeueOpponent(out Guid opponentId);
        void EnqueuePlayer(Guid playerId);
        void AddToActiveGames(Guid playerId, Guid opponentId);
        bool PlayerInGame(Guid playerId);
        bool PlayerInQueue(Guid playerId);
        Guid? GetOpponent(Guid playerId);
        void RemoveFromGame(Guid playerId, Guid opponentId);
        void RemoveFromWaitingQueue(Guid playerId);
    }
}
