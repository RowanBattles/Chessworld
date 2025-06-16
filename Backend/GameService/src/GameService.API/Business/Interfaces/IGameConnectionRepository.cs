using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;
namespace GameService.API.Business.Interfaces
{
    public interface IGameConnectionRepository
    {
        Task<GameConnectionModel?> GetGameConnectionAsync(Guid gameId);
        Task<Guid> GetGameConnectionByConnectionIdAsync(string connectionId);
        Task UpsertGameConnectionAsync(GameConnectionModel gameConnectionModel);
        Task AddConnectionAsync(string connectionId, string gameId, string? color);
        Task RemoveConnectionAsync(string connectionId, string gameId);
    }
}
