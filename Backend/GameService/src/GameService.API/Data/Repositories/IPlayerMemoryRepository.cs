namespace GameService.API.Data.Repositories
{
    public interface IPlayerMemoryRepository
    {
        void AddPlayer(Guid playerId, string connectionId);
        string? GetConnectionId(Guid playerId);
        Guid? GetPlayerId(string connectionId);
    }
}