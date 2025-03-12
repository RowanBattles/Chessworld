using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Matchmaking.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchmakingController : ControllerBase
    {
        private readonly IMatchmakingService _matchmakingService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MatchmakingController(IMatchmakingService matchmakingService, IHttpContextAccessor httpContextAccessor)
        {
            _matchmakingService = matchmakingService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("connect")]
        public IActionResult Connect()
        {
            var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is null");
            var connectionId = Guid.NewGuid().ToString(); // Generate a new connection ID
            string? playerId = context.Request.Cookies["PlayerId"];

            if (string.IsNullOrEmpty(playerId))
            {
                // Add anonymous player
                Guid newPlayerId = _matchmakingService.AddAnonyomousPlayer(connectionId);
                context.Response.Cookies.Append("PlayerId", newPlayerId.ToString(), new CookieOptions { HttpOnly = true, Expires = DateTime.UtcNow.AddYears(1) });
            }
            else
            {
                // Update connectionId for player
                _matchmakingService.UpdatePlayerConnectionId(playerId, connectionId);
            }

            return Ok();
        }

        [HttpPost("disconnect")]
        public IActionResult Disconnect()
        {
            // Handle disconnection
            return Ok();
        }

        [HttpPost("findgame")]
        public async Task<IActionResult> FindGame([FromBody] Guid playerId)
        {
            // Handle finding game
            return Ok();
        }

        [HttpPost("leavegame")]
        public async Task<IActionResult> LeaveGame([FromBody] Guid playerId)
        {
            // Handle leaving game
            return Ok();
        }
    }
}
