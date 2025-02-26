namespace GameService.API.src.Domain.DTOs
{
    public class FindGameResponseDto(string opponentId, bool isWaiting)
    {
        public string? OpponentId { get; private set; } = opponentId;
        public bool IsWaiting { get; private set; } = isWaiting;
    }
}