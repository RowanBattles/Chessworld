namespace GameService.API.Business.Models
{
    public class GameConnectionModel
    {
        public Guid GameId { get; private set; }
        public string? ConnectionWhite { get; set; }
        public string? ConnectionBlack { get; set; }
        public List<string> ConnectionSpectators { get; set; }

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
            else if (color == "black")
            {
                ConnectionBlack = connectionId;
            }
            else if (string.IsNullOrEmpty(color))
            {
                ConnectionSpectators.Add(connectionId);
            }
            else
            {
                throw new ArgumentException("Invalid color");
            }
        }

        public string? GetColorByConnectionId(string connectionId)
        {
            if (ConnectionWhite == connectionId)
            {
                return "white";
            }
            else if (ConnectionBlack == connectionId)
            {
                return "black";
            }
            else if (ConnectionSpectators.Contains(connectionId))
            {
                return null;
            }
            else
            {
                throw new ArgumentException("Connection ID not found");
            }
        }

        public (bool, bool, int) StatusResponse()
        {
            bool white = !string.IsNullOrEmpty(ConnectionWhite);
            bool black = !string.IsNullOrEmpty(ConnectionBlack);
            int spectators = ConnectionSpectators.Count;
            return (white, black, spectators);
        }

    }
}
