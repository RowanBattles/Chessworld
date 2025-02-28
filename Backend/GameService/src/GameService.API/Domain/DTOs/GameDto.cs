namespace GameService.API.Domain.DTOs
{
    public class Game
    {
        public Guid GameId { get; private set; }
        public Guid Player1Id { get; private set; }
        public Guid Player2Id { get; private set; }

        public Game(Guid player1Id, Guid player2Id)
        {
            GameId = Guid.NewGuid();
            Player1Id = player1Id;
            Player2Id = player2Id;
        }
    }
}
