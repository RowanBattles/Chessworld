namespace Matchmaking.API.API.Responses
{
    public class MatchStatusResponse
    {
        public bool MatchFound { get; private set; }
        public string? GameUrl { get; private set; }
        public string Message { get; private set; }

        public MatchStatusResponse(bool matchFound, string? gameUrl, string message)
        {
            MatchFound = matchFound;
            GameUrl = gameUrl;
            Message = message;
        }
    }
}
