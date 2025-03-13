using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;
using GameService.API.Contract.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameService.API.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] GameRequestModel gameRequestModel)
        {
            if (gameRequestModel == null)
            {
                return BadRequest("Game request model is null.");
            }

            var gameModel = GameMapper.ToGameModel(gameRequestModel);
            var gameId = await _gameService.CreateGame(gameModel);

            return Ok(gameId);
        }
    }
}
