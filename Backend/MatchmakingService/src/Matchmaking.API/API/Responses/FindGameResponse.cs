namespace Matchmaking.API.API.Responses
{
    public class FindGameResponse
    {
        public bool MatchFound { get; private set; }
        public string? GameId { get; private set; }
        public string Message { get; private set; }

        public FindGameResponse(bool matchFound, string? gameId, string message)
        {
            MatchFound = matchFound;
            GameId = gameId;
            Message = message;
        }
    }
}
