using GameService.API.Contract.Enums;

namespace GameService.API.Business.Models
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public string WhiteToken { get; private set; }
        public string BlackToken { get; private set; }
        public GameStatus Status { get; private set; }

        public GameModel(Guid id, string whiteToken, string blackToken, GameStatus status)
        {
            Id = id;
            WhiteToken = whiteToken;
            BlackToken = blackToken;
            Status = status;
        }
    }
}
