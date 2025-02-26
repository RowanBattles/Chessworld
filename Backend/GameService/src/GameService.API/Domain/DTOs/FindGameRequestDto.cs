namespace GameService.API.src.Domain.DTOs
{
    public class FindGameRequestDto(Guid playerId)
    {
        public Guid PlayerId { get; private set; } = playerId;
    }
}