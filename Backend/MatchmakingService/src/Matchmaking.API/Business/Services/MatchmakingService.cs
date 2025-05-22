using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Data.Interfaces;
using System.Text.Json;
using NanoidDotNet;

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

        public async Task<(bool, string, string?)> FindAndCreateGameAsync()
        {
            string playerToken = Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", size: 8);
            string? opponentToken = _matchmakingRepository.TryDequeuePlayer();

            if (opponentToken == null) 
            {
                _matchmakingRepository.EnqueuePlayer(playerToken);
                return (false, playerToken, null);
            }

            try
            {
                Guid gameId = await CreateGameAsync(playerToken, opponentToken);
                return (true, playerToken, gameId.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create game.");
                throw;
            }
        }

        public async Task<(bool, string?, string)> GetMatchStatus(string playerToken)
        {
            bool playerInQueue = _matchmakingRepository.GetFirstPlayerInQueue() == playerToken;

            if (playerInQueue)
            {
                return (false, null, "In queue");
            }

            try
            {
                Guid gameId = await GetGameUrlAsync(playerToken);

                if (gameId != Guid.Empty)
                {
                    return (true, gameId.ToString(), "Match found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get match status.");
            }

            return (false, null, "Player not found");
        }

        private async Task<Guid> CreateGameAsync(string whiteToken, string blackToken)
        {
            var requestUri = $"{_gameServiceBaseUrl}/games/create?whiteToken={whiteToken}&blackToken={blackToken}";
            _logger.LogInformation($"Creating game with URL: {requestUri}");

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
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing response from GameService.");
                throw;
            }
        }

        private async Task<Guid> GetGameUrlAsync(string playerToken)
        {
            var requestUri = $"{_gameServiceBaseUrl}/games/status?playerToken={playerToken}";
            _logger.LogInformation($"Getting game status with URL: {requestUri}");

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
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing response from GameService.");
                throw;
            }
        }
    }
}
