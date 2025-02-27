using GameService.API.src.Domain.DTOs;

namespace GameService.API.src.Business.Services
{
    public interface IGameService
    {
        Guid? DisconnectPlayer(Guid? playerId);
        FindGameResponseDto? MatchPlayer(Guid playerId);
        Guid? RemovePlayer(Guid playerId);
    }
}
