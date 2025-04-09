using GameService.API.Business.Interfaces;
using GameService.API.Business.Models;
using GameService.API.Business.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GameService.Tests.Business
{
    public class GameConnectionServiceTests
    {
        private readonly Mock<IGameConnectionRepository> _repositoryMock;
        private readonly Mock<ILogger<GameConnectionService>> _loggerMock;
        private readonly GameConnectionService _service;

        public GameConnectionServiceTests()
        {
            _repositoryMock = new Mock<IGameConnectionRepository>();
            _loggerMock = new Mock<ILogger<GameConnectionService>>();
            _service = new GameConnectionService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddConnectionAsync_ShouldAddNewConnection_WhenGameConnectionDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var connectionId = "connection1";
            var color = "white";

            _repositoryMock.Setup(repo => repo.GetGameConnectionAsync(gameId)).ReturnsAsync((GameConnectionModel?)null);

            // Act
            var result = await _service.AddConnectionAsync(gameId, connectionId, color);

            // Assert
            _repositoryMock.Verify(repo => repo.AddGameConnectionAsync(It.IsAny<GameConnectionModel>()), Times.Once);
            Assert.True(result.Item1); // White connected
        }

        [Fact]
        public async Task AddConnectionAsync_ShouldUpdateExistingConnection_WhenGameConnectionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var connectionId = "connection1";
            var color = "white";
            var existingConnection = new GameConnectionModel(gameId);

            _repositoryMock.Setup(repo => repo.GetGameConnectionAsync(gameId)).ReturnsAsync(existingConnection);

            // Act
            var result = await _service.AddConnectionAsync(gameId, connectionId, color);

            // Assert
            _repositoryMock.Verify(repo => repo.UpdateGameConnectionAsync(existingConnection), Times.Once);
            Assert.True(result.Item1); // White connected
        }

        [Fact]
        public async Task AddConnectionAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var connectionId = "connection1";
            var color = "white";

            _repositoryMock.Setup(repo => repo.GetGameConnectionAsync(gameId)).ThrowsAsync(new Exception("Database error"));

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
            await Assert.ThrowsAsync<Exception>(() => _service.AddConnectionAsync(gameId, connectionId, color));
            Assert.Contains("Error adding connection", logMessages);
        }

        [Fact]
        public async Task GetColorByConnectionId_ShouldReturnColor_WhenConnectionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var connectionId = "connection1";
            var expectedColor = "white";
            var gameConnection = new GameConnectionModel(gameId);
            gameConnection.AddConnection(connectionId, expectedColor);

            _repositoryMock.Setup(repo => repo.GetGameConnectionAsync(gameId)).ReturnsAsync(gameConnection);

            // Act
            var result = await _service.GetColorByConnectionId(gameId, connectionId);

            // Assert
            Assert.Equal(expectedColor, result);
        }

        [Fact]
        public async Task GetColorByConnectionId_ShouldThrowException_WhenGameConnectionNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var connectionId = "connection1";

            _repositoryMock.Setup(repo => repo.GetGameConnectionAsync(gameId)).ReturnsAsync((GameConnectionModel?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetColorByConnectionId(gameId, connectionId));
        }

        [Fact]
        public async Task GetGameIdByConnectionId_ShouldReturnGameId_WhenConnectionExists()
        {
            // Arrange
            var connectionId = "connection1";
            var expectedGameId = Guid.NewGuid();

            _repositoryMock.Setup(repo => repo.GetGameConnectionByConnectionIdAsync(connectionId)).ReturnsAsync(expectedGameId);

            // Act
            var result = await _service.GetGameIdByConnectionId(connectionId);

            // Assert
            Assert.Equal(expectedGameId, result);
        }

        [Fact]
        public async Task GetGameIdByConnectionId_ShouldThrowException_WhenGameIdIsEmpty()
        {
            // Arrange
            var connectionId = "connection1";

            _repositoryMock.Setup(repo => repo.GetGameConnectionByConnectionIdAsync(connectionId)).ReturnsAsync(Guid.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetGameIdByConnectionId(connectionId));
        }

        [Fact]
        public async Task GetGameIdByConnectionId_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var connectionId = "connection1";

            _repositoryMock.Setup(repo => repo.GetGameConnectionByConnectionIdAsync(connectionId)).ThrowsAsync(new Exception("Database error"));

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
            await Assert.ThrowsAsync<Exception>(() => _service.GetGameIdByConnectionId(connectionId));
            Assert.Contains("Error getting game ID by connection ID", logMessages);
        }
    }
}
