using GameService.API.Contract.Enums;

namespace GameService.API.Business.Models
{
    public class GameModel
    {
        public Guid Id { get; set; }
        public Guid White { get; private set; }
        public Guid Black { get; private set; }
        public GameStatus Status { get; private set; }

        public GameModel(Guid id, Guid white, Guid black, GameStatus status)
        {
            Id = id; 
            White = white;
            Black = black;
            Status = status;
        }
    }
}
