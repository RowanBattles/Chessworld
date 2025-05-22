namespace GameService.API.API.Responses
{
    public class GamePlayerResponse
    {
        public string GameId { get; private set; }
        public string WhiteId { get; private set; }
        public string BlackId { get; private set; }

        public GamePlayerResponse(string gameId, string whiteId, string blackId)
        {
            GameId = gameId;
            WhiteId = whiteId;
            BlackId = blackId;
        }
    }
}
