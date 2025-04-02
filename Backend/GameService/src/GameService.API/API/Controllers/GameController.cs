using GameService.API.API.Responses;
using GameService.API.Business.Interfaces;
using GameService.API.Contract.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GameService.API.API.Controllers
{
    [ApiController]
    [Route("/games")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromQuery] string whiteToken, [FromQuery] string blackToken)
        {
            var gameModel = GameMapper.ToGameModel(whiteToken, blackToken);
            try
            {
                await _gameService.CreateGame(gameModel);
                return Ok(gameModel.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating game");
                return StatusCode(StatusCodes.Status500InternalServerError, "Game could not be created");
            }
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetMatchStatus([FromQuery] string playerToken)
        {
            try
            {
                Guid gameUrl = await _gameService.GetStatusByPlayerId(playerToken);
                return Ok(gameUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game status");
                return StatusCode(StatusCodes.Status500InternalServerError, "Game status could not be retrieved");
            }
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGame([FromRoute] string gameId)
        {
            try
            {
                string? playerToken = HttpContext.Request.Cookies["playerToken"];
                (string status, string? validToken, string color) = await _gameService.GetGameByGameId(playerToken, gameId);
                GameResponse response = new(gameId, status, validToken, color);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, $"Game with ID {gameId} not found");
                return NotFound("Game not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error getting game with ID {gameId}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
            }
        }
    }
}
