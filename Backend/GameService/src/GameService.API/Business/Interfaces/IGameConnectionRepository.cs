using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;
namespace GameService.API.Business.Interfaces
{
    public interface IGameConnectionRepository
    {
        Task<GameConnectionModel?> GetGameConnectionAsync(Guid gameId);
        Task<Guid> GetGameConnectionByConnectionIdAsync(string connectionId);
        Task UpsertGameConnectionAsync(GameConnectionModel gameConnectionModel);
    }
}
