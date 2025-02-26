using GameService.API.src.Domain.DTOs;

namespace GameService.API.src.Business.Services
{
    public interface IGameService
    {
        FindGameResponseDto MatchPlayer(Guid playerId);
        Guid? RemovePlayer(Guid playerId);
    }
}
