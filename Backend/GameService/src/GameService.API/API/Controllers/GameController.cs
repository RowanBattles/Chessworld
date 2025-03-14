using GameService.API.Business.Interfaces;
using GameService.API.Contract.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameService.API.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame(Guid whiteId, Guid blackId)
        {
            var gameModel = GameMapper.ToGameModel(whiteId, blackId);
            try
            {
                await _gameService.CreateGameAsync(gameModel);
                return Ok(gameModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game");
                return StatusCode(StatusCodes.Status500InternalServerError, "Game could not be created");
            }
        }
    }
}
