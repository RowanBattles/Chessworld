namespace Matchmaking.API.API.Responses
{
    public class MatchStatusResponse
    {
        public bool MatchFound { get; private set; }
        public string? GameId { get; private set; }
        public string Message { get; private set; }

        public MatchStatusResponse(bool matchFound, string? gameUrl, string message)
        {
            MatchFound = matchFound;
            GameId = gameUrl;
            Message = message;
        }
    }
}
