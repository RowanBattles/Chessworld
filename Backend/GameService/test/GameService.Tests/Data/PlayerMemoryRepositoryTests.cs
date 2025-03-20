using System;
using Xunit;
using GameService.API.Data.Repositories;

namespace GameService.Tests.Data
{
    public class PlayerMemoryRepositoryTests
    {
        private readonly PlayerMemoryRepository _repository;

        public PlayerMemoryRepositoryTests()
        {
            _repository = new PlayerMemoryRepository();
        }

        [Fact]
        public void AddPlayer_ShouldAddPlayer()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var connectionId = "connection1";

            // Act
            _repository.AddPlayer(playerId, connectionId);

            // Assert
            var result = _repository.GetConnectionId(playerId);
            Assert.Equal(connectionId, result);
        }

        [Fact]
        public void RemovePlayer_ShouldRemovePlayer()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var connectionId = "connection1";
            _repository.AddPlayer(playerId, connectionId);

            // Act
            _repository.RemovePlayer(connectionId);

            // Assert
            var result = _repository.GetConnectionId(playerId);
            Assert.Null(result);
        }

        [Fact]
        public void GetConnectionId_ShouldReturnConnectionId()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var connectionId = "connection1";
            _repository.AddPlayer(playerId, connectionId);

            // Act
            var result = _repository.GetConnectionId(playerId);

            // Assert
            Assert.Equal(connectionId, result);
        }

        [Fact]
        public void GetConnectionId_ShouldReturnNullIfPlayerNotFound()
        {
            // Arrange
            var playerId = Guid.NewGuid();

            // Act
            var result = _repository.GetConnectionId(playerId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetPlayerId_ShouldReturnPlayerId()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var connectionId = "connection1";
            _repository.AddPlayer(playerId, connectionId);

            // Act
            var result = _repository.GetPlayerId(connectionId);

            // Assert
            Assert.Equal(playerId, result);
        }

        [Fact]
        public void GetPlayerId_ShouldReturnNullIfConnectionNotFound()
        {
            // Arrange
            var connectionId = "connection1";

            // Act
            var result = _repository.GetPlayerId(connectionId);

            // Assert
            Assert.Null(result);
        }
    }
}
