namespace GameService.API.Data.Repositories
{
    public interface IPlayerRepository
    {
        void AddPlayer(Guid playerId, string connectionId);
        void RemovePlayer(string connectionId);
        string? GetConnectionId(Guid playerId);
        Guid? GetPlayerId(string connectionId);
    }
}