namespace GameService.API.Business.Services
{
    public interface IPlayerService
    {
        Guid AddConnectionIdToPlayer(string connectionId);
        string? GetConnectionId(Guid playerId);
        Guid? GetPlayerId(string connectionId);
    }
}