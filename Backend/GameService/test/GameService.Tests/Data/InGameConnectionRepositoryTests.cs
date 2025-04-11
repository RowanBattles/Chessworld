using GameService.API.Data.Repository;
using GameService.API.Business.Models;
using Microsoft.Extensions.Logging;
using Moq;

namespace GameService.Tests.Data.Repository
{
    public class InGameConnectionRepositoryTests
    {
        private readonly InGameConnectionRepository _repository;
        private readonly Mock<ILogger<InGameConnectionRepository>> _loggerMock;

        public InGameConnectionRepositoryTests()
        {
            _loggerMock = new Mock<ILogger<InGameConnectionRepository>>();
            _repository = new InGameConnectionRepository(_loggerMock.Object);
        }

        [Fact]
        public async Task GetGameConnectionAsync_ShouldReturnConnection_WhenConnectionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameConnection = new GameConnectionModel(gameId, "whiteConnection", "blackConnection", new List<string>());
            await _repository.AddGameConnectionAsync(gameConnection);

            // Act
            var result = await _repository.GetGameConnectionAsync(gameId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameId, result!.GameId);
        }

        [Fact]
        public async Task GetGameConnectionAsync_ShouldReturnNull_WhenConnectionDoesNotExist()
        {
            // Act
            var result = await _repository.GetGameConnectionAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddGameConnectionAsync_ShouldAddConnection_WhenConnectionDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameConnection = new GameConnectionModel(gameId, "whiteConnection", "blackConnection", new List<string>());

            // Act
            await _repository.AddGameConnectionAsync(gameConnection);

            // Assert
            var addedConnection = await _repository.GetGameConnectionAsync(gameId);
            Assert.NotNull(addedConnection);
            Assert.Equal(gameId, addedConnection!.GameId);
        }

        [Fact]
        public async Task AddGameConnectionAsync_ShouldThrowException_WhenConnectionAlreadyExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameConnection = new GameConnectionModel(gameId, "whiteConnection", "blackConnection", new List<string>());
            await _repository.AddGameConnectionAsync(gameConnection);

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
            await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.AddGameConnectionAsync(gameConnection));
            Assert.Contains("Game connection already exists for gameId: " + gameId, logMessages);
        }

        [Fact]
        public async Task UpdateGameConnectionAsync_ShouldUpdateConnection_WhenConnectionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var initialConnection = new GameConnectionModel(gameId, "whiteConnection", "blackConnection", new List<string>());
            await _repository.AddGameConnectionAsync(initialConnection);

            var updatedConnection = new GameConnectionModel(gameId, "newWhiteConnection", "newBlackConnection", new List<string> { "spectator1" });

            // Act
            await _repository.UpdateGameConnectionAsync(updatedConnection);

            // Assert
            var result = await _repository.GetGameConnectionAsync(gameId);
            Assert.NotNull(result);
            Assert.Equal("newWhiteConnection", result!.ConnectionWhite);
            Assert.Equal("newBlackConnection", result.ConnectionBlack);
            Assert.Contains("spectator1", result.ConnectionSpectators);
        }

        [Fact]
        public async Task GetGameConnectionByConnectionIdAsync_ShouldReturnGameId_WhenConnectionExists()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameConnection = new GameConnectionModel(gameId, "whiteConnection", "blackConnection", new List<string> { "spectator1" });
            await _repository.AddGameConnectionAsync(gameConnection);

            // Act
            var result = await _repository.GetGameConnectionByConnectionIdAsync("whiteConnection");

            // Assert
            Assert.Equal(gameId, result);
        }

        [Fact]
        public async Task GetGameConnectionByConnectionIdAsync_ShouldReturnEmptyGuid_WhenConnectionDoesNotExist()
        {
            // Act
            var result = await _repository.GetGameConnectionByConnectionIdAsync("nonExistentConnection");

            // Assert
            Assert.Equal(Guid.Empty, result);
        }
    }
}
