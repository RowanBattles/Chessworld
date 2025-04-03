namespace GameService.API.Business.Models
{
    public class GameConnectionModel
    {
        public Guid GameId { get; private set; }
        public string? ConnectionWhite { get; private set; }
        public string? ConnectionBlack { get; private set; }
        public List<string> ConnectionSpectators { get; private set; }

        public GameConnectionModel(Guid gameId)
        {
            GameId = gameId;
            ConnectionSpectators = [];
        }

        public GameConnectionModel(Guid gameId, string? connectionWhite, string? connectionBlack, List<string> connectionSpectators)
        {
            GameId = gameId;
            ConnectionWhite = connectionWhite;
            ConnectionBlack = connectionBlack;
            ConnectionSpectators = connectionSpectators;
        }

        public void AddConnection(string connectionId, string? color)
        {
            if (color == "white")
            {
                ConnectionWhite = connectionId;
            }
            if (color == "black")
            {
                ConnectionBlack = connectionId;
            }
            if (string.IsNullOrEmpty(color))
            {
                ConnectionSpectators.Add(connectionId);
            }
            else
            {
                throw new ArgumentException("Invalid color");
            }
        }
    }
}
