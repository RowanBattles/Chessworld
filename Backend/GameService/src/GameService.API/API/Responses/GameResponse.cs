
using System.Xml;

namespace GameService.API.API.Responses
{
    public class GameResponse
    {
        public GameClass Game { get; private set; }
        public PlayerClass Player { get; private set; }
        public OpponentClass Opponent { get; private set; }

        public GameResponse(string gameId, string status, string fen, string? token, string color)
        {
            Game = new GameClass(gameId, status, fen);
            (Player, Opponent) = PlayerAssigningLogic(token, color);
        }

        private static (PlayerClass Player, OpponentClass Opponent) PlayerAssigningLogic(string? token, string color)
        {
            bool isSpectator = token == null;
            PlayerClass player = new(token, color, isSpectator);
            OpponentClass opponent =new(ToggleColor(player.Color));
            return (player, opponent);
        }

        private static string ToggleColor(string color)
        {
            if (color == "white")
            {
                return "black";
            }
            if (color == "black")
            {
                return "white";
            }
            throw new ArgumentException("Invalid color value. Expected 'white' or 'black'.", nameof(color));
        }

        public class GameClass
        {
            public string GameId { get; private set; }
            public string Status { get; private set; }
            public string Fen { get; private set; }

            public GameClass(string gameId, string status, string fen)
            {
                GameId = gameId;
                Status = status;
                Fen = fen;
            }
        }

        public class PlayerClass
        {
            public string? Id { get; private set; }
            public string Color { get; private set; }
            public bool IsSpectator { get; private set; }

            public PlayerClass(string? id, string color, bool isSpectator)
            {
                Id = id;
                Color = color;
                IsSpectator = isSpectator;
            }
        }

        public class OpponentClass
        {
            public string Color { get; private set; }

            public OpponentClass(string color)
            {
                Color = color;
            }
        }
    }
}
