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
                _matchmakingRepository.DequeuePlayer((Guid)opponentId);
                return (true, gameId.ToString());
            }
            else
            {
                _matchmakingRepository.EnqueuePlayer(playerId);
                return (false, playerId.ToString());
            }
        }

        public async Task<(bool, string?, string)> GetMatchStatus(string playerId)
        {
            // player in queue?
            bool playerInQueue = _matchmakingRepository.GetFirstPlayerInQueue() == Guid.Parse(playerId);

            if (playerInQueue)
            {
                return (false, null, "In queue");
            }

            // player in game?
            Guid gameUrl = await GetGameUrlAsync(Guid.Parse(playerId));

            if (gameUrl != Guid.Empty)
            {
                return (true, gameUrl.ToString(), "Match found");
            }

            // not found
            return (false, null, "Player not found");
        }

        private async Task<Guid> CreateGameAsync(Guid whiteId, Guid blackId)
        {
            var requestUri = $"{_gameServiceBaseUrl}/games/create?whiteId={whiteId}&blackId={blackId}";
            _logger.LogInformation($"gameServiceBaseUrl: {_gameServiceBaseUrl}");
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

        private async Task<Guid> GetGameUrlAsync(Guid playerId)
        {
            var requestUri = $"{_gameServiceBaseUrl}/games/status?playerId={playerId}";
            _logger.LogInformation($"gameServiceBaseUrl: {_gameServiceBaseUrl}");
            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Guid>(responseString);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling GameService to get game URL.");
                throw;
            }
        }
    }
}
