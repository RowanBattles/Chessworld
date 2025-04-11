using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;
using GameService.API.Contract.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameService.Tests.Business
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ILogger<GameService.API.Business.Services.GameService>> _loggerMock;
        private readonly IGameService _gameService;

        public GameServiceTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _loggerMock = new Mock<ILogger<GameService.API.Business.Services.GameService>>();
            _gameService = new GameService.API.Business.Services.GameService(_gameRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CreateGame_ShouldAddGameToRepository()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken");

            // Act
            await _gameService.CreateGame(gameModel);

            // Assert
            _gameRepositoryMock.Verify(repo => repo.AddGame(gameModel), Times.Once);
        }

        [Fact]
        public async Task CreateGame_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken");
            _gameRepositoryMock.Setup(repo => repo.AddGame(It.IsAny<GameModel>())).ThrowsAsync(new Exception("Database error"));

            var logMessages = new List<string>();
            _loggerMock
                .Setup(logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Callback<LogLevel, EventId, object, Exception, Delegate>((logLevel, eventId, state, exception, formatter) =>
                {
                    logMessages.Add(state.ToString()!);
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _gameService.CreateGame(gameModel));
            Assert.Equal("Game could not be created", exception.Message);

            Assert.Contains("Error adding game to repository", logMessages);
        }

        [Fact]
        public async Task GetStatusByPlayerId_ShouldReturnGameId()
        {
            // Arrange
            var playerToken = "playerToken";
            var gameId = Guid.NewGuid();
            _gameRepositoryMock.Setup(repo => repo.GetGameByPlayerId(playerToken)).ReturnsAsync(gameId);

            // Act
            var result = await _gameService.GetStatusByPlayerId(playerToken);

            // Assert
            Assert.Equal(gameId, result);
        }

        [Fact]
        public async Task GetStatusByPlayerId_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var playerToken = "playerToken";
            _gameRepositoryMock.Setup(repo => repo.GetGameByPlayerId(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            var logMessages = new List<string>();
            _loggerMock
                .Setup(logger => logger.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()))
                .Callback<LogLevel, EventId, object, Exception, Delegate>((logLevel, eventId, state, exception, formatter) =>
                {
                    logMessages.Add(state.ToString()!);
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _gameService.GetStatusByPlayerId(playerToken));
            Assert.Equal("Game could not be retrieved", exception.Message);

            Assert.Contains("Error retrieving game for playerToken", logMessages);
        }


        [Fact]
        public async Task GetGameByGameId_ShouldReturnGameDetails()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var playerToken = "whiteToken";
            var gameModel = new GameModel(gameId, "whiteToken", "blackToken", GameStatus.InProgress, "initialFen");
            _gameRepositoryMock.Setup(repo => repo.GetGameByGameId(gameId)).ReturnsAsync(gameModel);

            // Act
            var result = await _gameService.GetGameByGameId(playerToken, gameId);

            // Assert
            Assert.Equal("InProgress", result.status);
            Assert.Equal("initialFen", result.fen);
            Assert.Equal("whiteToken", result.token);
            Assert.Equal("white", result.color);
        }

        [Fact]
        public async Task GetGameByGameId_ShouldThrowKeyNotFoundException_WhenGameNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _gameRepositoryMock.Setup(repo => repo.GetGameByGameId(gameId)).ReturnsAsync((GameModel?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameByGameId(null, gameId));
        }

        [Fact]
        public async Task MakeMove_ShouldUpdateGameWithNewFen()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var color = "white";
            var uci = "e2e4";
            var gameModel = new GameModel(gameId, "whiteToken", "blackToken");
            var firstFen = gameModel.Fen;
            _gameRepositoryMock.Setup(repo => repo.GetGameByGameId(gameId)).ReturnsAsync(gameModel);
            _gameRepositoryMock.Setup(repo => repo.UpdateGame(It.IsAny<GameModel>())).ReturnsAsync(true);

            // Act
            var result = await _gameService.MakeMove(gameId, color, uci);

            // Assert
            _gameRepositoryMock.Verify(repo => repo.UpdateGame(It.IsAny<GameModel>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(gameModel.Fen, result);
            Assert.NotEqual(firstFen, result);
        }

        [Fact]
        public async Task MakeMove_ShouldThrowArgumentException_WhenGameIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.MakeMove(Guid.Empty, "white", "e2e4"));
        }

        [Fact]
        public async Task MakeMove_ShouldThrowArgumentException_WhenColorIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.MakeMove(Guid.NewGuid(), null, "e2e4"));
        }

        [Fact]
        public async Task MakeMove_ShouldThrowArgumentException_WhenUciIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _gameService.MakeMove(Guid.NewGuid(), "white", "e2"));
        }
    }
}
