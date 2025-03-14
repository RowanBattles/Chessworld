using Matchmaking.API.API.Responses;
using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Business.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Matchmaking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                var (matchFound, Id) = await _matchmakingService.FindAndCreateGameAsync();
                var response = new FindGameResponse(matchFound, matchFound ? Id : null, !matchFound ? Id : null, matchFound ? "Match found" : "Put in queue");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while finding a game.");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("matchstatus/{playerId}")]
        public IActionResult GetMatchStatus([FromRoute] string playerId)
        {
            if (!Guid.TryParse(playerId, out _))
            {
                return BadRequest("Invalid player ID format.");
            }

            try
            {
                string? gameUrl = _matchmakingService.GetMatchStatus(playerId);
                var response = new MatchStatusResponse(gameUrl != null, gameUrl ?? "In queue");
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
