namespace Matchmaking.API.API.Responses
{
    public class FindGameResponse
    {
        public bool MatchFound { get; private set; }
        public string? GameId { get; private set; }
        public string? PlayerId { get; private set; }
        public string Message { get; private set; }

        public FindGameResponse(bool matchFound, string? gameId, string? playerId, string message)
        {
            MatchFound = matchFound;
            GameId = gameId;
            PlayerId = playerId;
            Message = message;
        }
    }
}
