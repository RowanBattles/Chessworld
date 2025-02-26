namespace GameService.API.src.Domain.DTOs
{
    public class FindGameResponseDto(Guid opponentId, bool isWaiting)
    {
        public Guid? OpponentId { get; private set; } = opponentId;
        public bool IsWaiting { get; private set; } = isWaiting;
    }
}