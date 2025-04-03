using GameService.API.Business.Models;
using GameService.API.Contract.Mappers;
namespace GameService.API.Business.Interfaces
{
    public interface IGameConnectionRepository
    {
        Task<GameConnectionModel?> GetGameConnectionAsync(Guid gameId);
        Task AddGameConnectionAsync(GameConnectionModel gameConnectionModel);
        Task UpdateGameConnectionAsync(GameConnectionModel gameConnectionModel);
    }
}
