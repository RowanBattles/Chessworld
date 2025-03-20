using Matchmaking.API.API.Responses;
using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Matchmaking.API.API.Controllers
{
    [ApiController]
    [Route("/matchmaking")]
    public class MatchmakingController : ControllerBase
    {
        private readonly IMatchmakingService _matchmakingService;
        private readonly ILogger<MatchmakingController> _logger;

        public MatchmakingController(IMatchmakingService matchmakingService, ILogger<MatchmakingController> logger)
        {
            _matchmakingService = matchmakingService;
            _logger = logger;
        }

        [HttpPost("findgame")]
        public async Task<IActionResult> FindGame()
        {
            try
            {
                var (matchFound, playerToken, gameId) = await _matchmakingService.FindAndCreateGameAsync();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddHours(2)
                };
                Response.Cookies.Append("playerToken", playerToken, cookieOptions);

                var response = new FindGameResponse(matchFound, gameId, message: matchFound ? "Match found" : "Put in queue");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding a game.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("matchstatus")]
        public async Task<IActionResult> GetMatchStatus()
        {
            if (!Request.Cookies.TryGetValue("playerToken", out var playerToken) || string.IsNullOrEmpty(playerToken) || playerToken.Length != 8)
            {
                return BadRequest("Invalid or missing player token. It must be a NanoID with a size of 8.");
            }

            try
            {
                var (matchFound, gameUrl, message) = await _matchmakingService.GetMatchStatus(playerToken);
                var response = new MatchStatusResponse(matchFound, gameUrl, message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting match status.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
