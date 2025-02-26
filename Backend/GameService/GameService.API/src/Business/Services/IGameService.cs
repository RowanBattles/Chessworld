using GameService.API.src.Domain.DTOs;

namespace GameService.API.src.Business.Services
{
    public interface IGameService
    {
        FindGameResponseDto MatchPlayer(string playerId);
        string? RemovePlayer(string playerId);
    }
}
