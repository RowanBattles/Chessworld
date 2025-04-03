

namespace GameService.API.Business.Interfaces
{
    public interface IGameConnectionService
    {
        Task AddConnectionAsync(Guid gameId, string connectionId, string? color);
    }
}
