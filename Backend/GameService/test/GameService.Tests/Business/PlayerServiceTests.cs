using System;
using Xunit;
using Moq;
using GameService.API.Business.Services;

namespace GameService.Tests.Business
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerService> _playerServiceMock;

        public PlayerServiceTests()
        {
            _playerServiceMock = new Mock<IPlayerService>();
        }

        [Fact]
        public void AddConnectionId_ShouldReturnGuid()
        {
            // Arrange
            var connectionId = "test-connection-id";
            var expectedGuid = Guid.NewGuid();
            _playerServiceMock.Setup(service => service.AddConnectionId(connectionId)).Returns(expectedGuid);

            // Act
            var result = _playerServiceMock.Object.AddConnectionId(connectionId);

            // Assert
            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public void RemoveConnectionId_ShouldReturnGuid_WhenConnectionIdExists()
        {
            // Arrange
            var connectionId = "test-connection-id";
            var expectedGuid = Guid.NewGuid();
            _playerServiceMock.Setup(service => service.RemoveConnectionId(connectionId)).Returns(expectedGuid);

            // Act
            var result = _playerServiceMock.Object.RemoveConnectionId(connectionId);

            // Assert
            Assert.Equal(expectedGuid, result);
        }

        [Fact]
        public void RemoveConnectionId_ShouldReturnNull_WhenConnectionIdDoesNotExist()
        {
            // Arrange
            var connectionId = "non-existent-connection-id";
            _playerServiceMock.Setup(service => service.RemoveConnectionId(connectionId)).Returns((Guid?)null);

            // Act
            var result = _playerServiceMock.Object.RemoveConnectionId(connectionId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetConnectionId_ShouldReturnConnectionId_WhenPlayerIdExists()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var expectedConnectionId = "test-connection-id";
            _playerServiceMock.Setup(service => service.GetConnectionId(playerId)).Returns(expectedConnectionId);

            // Act
            var result = _playerServiceMock.Object.GetConnectionId(playerId);

            // Assert
            Assert.Equal(expectedConnectionId, result);
        }

        [Fact]
        public void GetConnectionId_ShouldReturnNull_WhenPlayerIdDoesNotExist()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            _playerServiceMock.Setup(service => service.GetConnectionId(playerId)).Returns((string?)null);

            // Act
            var result = _playerServiceMock.Object.GetConnectionId(playerId);

            // Assert
            Assert.Null(result);
        }
    }
}