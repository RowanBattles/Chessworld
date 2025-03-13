using System.ComponentModel.DataAnnotations;

namespace GameService.API.API.Responses
{
    public class GameRequestModel
    {
        public Guid GameId { get; private set; }
        public Guid Player1Id { get; private set; }
        public Guid Player2Id { get; private set; }
        public string Player1Color { get; private set; }
        public string Player2Color { get; private set; }

        public GameRequestModel(Guid gameId, Guid player1Id, Guid player2Id, string player1Color, string player2Color)
        {
            GameId = gameId;
            Player1Id = player1Id;
            Player2Id = player2Id;
            Player1Color = player1Color;
            Player2Color = player2Color;
        }
    }
}
