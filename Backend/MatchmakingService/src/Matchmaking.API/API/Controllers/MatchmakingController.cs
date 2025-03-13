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

        public MatchmakingController(IMatchmakingService matchmakingService)
        {
            _matchmakingService = matchmakingService;
        }

        [HttpPost("findgame")]
        public async Task<IActionResult> FindGame()
        {
            var (matchFound, gameUrl) = await _matchmakingService.FindAndCreateGameA();

            if (matchFound)
            {
                // return cookie and gameid?
            }
            else
            {
                // return ok in Queue
            }

            return null;
        }
    }
}
