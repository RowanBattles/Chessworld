namespace GameService.API.src.Domain.DTOs
{
    public class FindGameResponseDto
    {
        public Guid OpponentId { get; private set; }

        public FindGameResponseDto(Guid opponentId)
        {
            OpponentId = opponentId;
        }
    }
}