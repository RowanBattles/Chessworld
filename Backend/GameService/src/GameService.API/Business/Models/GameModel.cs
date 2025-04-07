using GameService.API.Contract.Enums;

namespace GameService.API.Business.Models
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public string WhiteToken { get; private set; }
        public string BlackToken { get; private set; }
        public GameStatus Status { get; private set; }
        public string Fen { get; private set; }

        public GameModel(Guid id, string whiteToken, string blackToken)
        {
            Id = id;
            WhiteToken = whiteToken;
            BlackToken = blackToken;
            Status = GameStatus.InProgress;
            Fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        }

        public GameModel(Guid id, string whiteToken, string blackToken, GameStatus status, string fen) : this(id, whiteToken, blackToken)
        {
            Id = id;
            WhiteToken = whiteToken;
            BlackToken = blackToken;
            Status = status;
            Fen = fen;
        }

        public void UpdateBoard(string fen)
        {
            Fen = fen;
        }
    }
}
