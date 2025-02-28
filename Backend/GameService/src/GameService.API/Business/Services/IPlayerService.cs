namespace GameService.API.Business.Services
{
    public interface IPlayerService
    {
        Guid AddConnectionId(string connectionId);
        Guid? RemoveConnectionId(string connectionId);
        string? GetConnectionId(Guid playerId);
    }
}