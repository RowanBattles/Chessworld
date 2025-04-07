

using GameService.API.Business.Models;

namespace GameService.API.Business.Interfaces
{
    public interface IGameConnectionService
    {
        Task<(bool white, bool black, int watchers)> AddConnectionAsync(Guid gameId, string connectionId, string? color);
        Task<string?> GetColorByConnectionId(Guid gameId, string connectionId);
        Task<Guid> GetGameIdByConnectionId(string connectionId);
    }
}
