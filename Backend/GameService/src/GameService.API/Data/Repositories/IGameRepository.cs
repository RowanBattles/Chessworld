using GameService.API.Domain.DTOs;
using System.Collections.Concurrent;

namespace GameService.API.src.Data.Repositories
{
    public interface IGameRepository
    {
        void EnqueuePlayer(Guid playerId);
        void DequeuePlayer(Guid playerId);
        bool PlayerInQueue(Guid playerId);
        Guid? MatchWithFirstPersonInQueue();
        bool PlayerInGame(Guid playerId);
        void AddGame(Game game);
        bool RemoveGame(Guid gameId);
        Game? GetGameByPlayerId(Guid playerId);
    }
}