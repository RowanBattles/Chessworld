using GameService.API.API.Controllers;
using GameService.API.Business.Interfaces;
using GameService.API.Contract.Mappers;
using GameService.API.API.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using GameService.API.Business.Models;

namespace GameService.Tests.API
{
    public class GameControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly Mock<ILogger<GameController>> _mockLogger;
        private readonly GameController _controller;

        public GameControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _mockLogger = new Mock<ILogger<GameController>>();
            _controller = new GameController(_mockGameService.Object, _mockLogger.Object);
        }

        private void SetupHttpContextWithCookies(string playerToken)
        {
            var httpContext = new DefaultHttpContext();
            var cookies = new Mock<IRequestCookieCollection>();
            cookies.Setup(c => c["playerToken"]).Returns(playerToken);
            httpContext.Request.Cookies = cookies.Object;
            _controller.ControllerContext.HttpContext = httpContext;
        }

        [Fact]
        public async Task CreateGame_ReturnsOk_WithValidGameId()
        {
            // Arrange
            var whiteToken = "whiteToken";
            var blackToken = "blackToken";
            _mockGameService.Setup(s => s.CreateGame(It.IsAny<GameModel>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateGame(whiteToken, blackToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True(Guid.TryParse(okResult.Value?.ToString(), out _));
        }

        [Fact]
        public async Task CreateGame_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var whiteToken = "whiteToken";
            var blackToken = "blackToken";
            _mockGameService.Setup(s => s.CreateGame(It.IsAny<GameModel>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.CreateGame(whiteToken, blackToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetMatchStatus_ReturnsOk_WithGameUrl()
        {
            // Arrange
            var playerToken = "playerToken";
            var gameUrl = Guid.NewGuid();
            _mockGameService.Setup(s => s.GetStatusByPlayerId(playerToken)).ReturnsAsync(gameUrl);

            // Act
            var result = await _controller.GetMatchStatus(playerToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(gameUrl, okResult.Value);
        }

        [Fact]
        public async Task GetMatchStatus_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var playerToken = "playerToken";
            _mockGameService.Setup(s => s.GetStatusByPlayerId(playerToken)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetMatchStatus(playerToken);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetGame_ReturnsOk_WithGameResponse()
        {
            // Arrange
            var gameId = Guid.NewGuid().ToString();
            var playerToken = "playerToken";
            var gameData = ("InProgress", "someFen", "validToken", "white");

            _mockGameService.Setup(s => s.GetGameByGameId(playerToken, Guid.Parse(gameId)))
                .ReturnsAsync(gameData);

            SetupHttpContextWithCookies(playerToken);

            // Act
            var result = await _controller.GetGame(gameId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<GameResponse>(okResult.Value);
            Assert.Equal(gameId, response.Game.GameId);
            Assert.Equal(gameData.Item1, response.Game.Status);
            Assert.Equal(gameData.Item2, response.Game.Fen);
            Assert.Equal(gameData.Item3, response.Player.Id);
            Assert.Equal(gameData.Item4, response.Player.Color);
        }

        [Fact]
        public async Task GetGame_ReturnsBadRequest_OnInvalidGameId()
        {
            // Arrange
            var invalidGameId = "invalid-guid";

            // Act
            var result = await _controller.GetGame(invalidGameId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid game ID format", badRequestResult.Value);
        }

        [Fact]
        public async Task GetGame_ReturnsNotFound_OnKeyNotFoundException()
        {
            // Arrange
            var gameId = Guid.NewGuid().ToString();
            var playerToken = "playerToken";

            _mockGameService.Setup(s => s.GetGameByGameId(playerToken, Guid.Parse(gameId)))
                .ThrowsAsync(new KeyNotFoundException());

            SetupHttpContextWithCookies(playerToken);

            // Act
            var result = await _controller.GetGame(gameId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Game not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetGame_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var gameId = Guid.NewGuid().ToString();
            var playerToken = "playerToken";

            _mockGameService.Setup(s => s.GetGameByGameId(playerToken, Guid.Parse(gameId)))
                .ThrowsAsync(new Exception());

            SetupHttpContextWithCookies(playerToken);

            // Act
            var result = await _controller.GetGame(gameId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }
}
