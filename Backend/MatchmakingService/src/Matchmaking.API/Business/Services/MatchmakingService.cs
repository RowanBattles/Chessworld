using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Data.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Matchmaking.API.Business.Services
{
    public class MatchmakingService : IMatchmakingService
    {
        private readonly IMatchmakingRepository _matchmakingRepository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<MatchmakingService> _logger;
        private readonly string _gameServiceBaseUrl;

        public MatchmakingService(IMatchmakingRepository matchmakingRepository, HttpClient httpClient, ILogger<MatchmakingService> logger, IConfiguration configuration)
        {
            _matchmakingRepository = matchmakingRepository;
            _httpClient = httpClient;
            _logger = logger;
            _gameServiceBaseUrl = $"http://{configuration["DownstreamServices:GameService:Host"]}:{configuration["DownstreamServices:GameService:Port"]}";
        }

        public async Task<(bool, string)> FindAndCreateGameAsync()
        {
            var playerId = Guid.NewGuid();
            Guid? opponentId = _matchmakingRepository.GetFirstPlayerInQueue();

            if (opponentId != null)
            {
                Guid gameId = await CreateGameAsync(playerId, (Guid)opponentId);
                // get succesfull response
                _matchmakingRepository.DequeuePlayer((Guid)opponentId);
                return (true, gameId.ToString());
            }
            else
            {
                _matchmakingRepository.EnqueuePlayer(playerId);
                return (false, playerId.ToString());
            }
        }

        public string? GetMatchStatus(string playerId)
        {
            // Logic
            return string.Empty;
        }

        private async Task<Guid> CreateGameAsync(Guid whiteId, Guid blackId)
        {
            var requestUri = $"{_gameServiceBaseUrl}/api/games?whiteId={whiteId}&blackId={blackId}";

            try
            {
                var response = await _httpClient.PostAsync(requestUri, null);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Guid>(responseString);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling GameService to create game.");
                throw;
            }
        }
    }
}
