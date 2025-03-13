using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Data.Interfaces;

namespace Matchmaking.API.Business.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly IMatchmakingRepository _matchmakingRepository;
        private readonly HttpClient _httpClient;

        public MatchmakingService(IMatchmakingRepository matchmakingRepository, HttpClient httpClient)
        {
            _matchmakingRepository = matchmakingRepository;
            _httpClient = httpClient;
        }

        public async Task<(bool matchFound, string? gameUrl)> FindAndCreateGameA()
        {
            var playerId = Guid.NewGuid();
            Guid? opponentId = _matchmakingRepository.GetFirstPlayerInQueue();

            if (opponentId != null)
            {
                var gameUrl = await CreateGameAsync(playerId, (Guid)opponentId);
                return (true, gameUrl);
            }
            else
            {
                _matchmakingRepository.EnqueuePlayer(playerId);
                return (false, null);
            }
        }     

        private async Task<string> CreateGameAsync(Guid White, Guid Black)
        {
            // make logic
            //var response = await _httpClient.PostAsync("https://other-microservice/api/games",
            //    new StringContent(JsonSerializer.Serialize(requestContent), System.Text.Encoding.UTF8, "application/json"));

            //response.EnsureSuccessStatusCode();

            //var responseContent = await response.Content.ReadAsStringAsync();
            //var gameResponse = JsonSerializer.Deserialize<GameResponse>(responseContent);

            return string.Empty;
        }
    }
}
