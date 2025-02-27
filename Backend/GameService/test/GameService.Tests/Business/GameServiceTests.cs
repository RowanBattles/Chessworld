using Xunit;
using Moq;
using GameService.API.src.Data.Repositories;
using GameService.API.src.Domain.DTOs;
using System;

namespace GameService.Tests.Business
{
    public class GameServiceTests
    {
        private readonly Mock<IGameRepository> _mockRepo;
        private readonly GameService.API.Business.Services.GameService _gameService;

        public GameServiceTests()
        {
            _mockRepo = new Mock<IGameRepository>();
            _gameService = new GameService.API.Business.Services.GameService(_mockRepo.Object);
        }

        [Fact]
        public void MatchPlayer_ShouldReturnWaiting_WhenNoMatchFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.TryDequeueOpponent(out It.Ref<Guid>.IsAny))
                     .Returns(false);
            Guid playerId = Guid.NewGuid();

            // Act
            var result = _gameService.MatchPlayer(playerId);

            // Assert
            Assert.Null(result);
            _mockRepo.Verify(repo => repo.EnqueuePlayer(playerId), Times.Once);
        }

        [Fact]
        public void MatchPlayer_ShouldReturnOpponent_WhenMatchFound()
        {
            // Arrange
            Guid opponentId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.TryDequeueOpponent(out opponentId))
                     .Returns(true);
            Guid playerId = Guid.NewGuid();

            // Act
            var result = _gameService.MatchPlayer(playerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(opponentId, result.OpponentId);
            _mockRepo.Verify(repo => repo.AddToActiveGames(playerId, opponentId), Times.Once);
        }

        [Fact]
        public void RemovePlayer_ShouldNotifyOpponent_WhenInGame()
        {
            // Arrange
            Guid playerId = Guid.NewGuid();
            Guid opponentId = Guid.NewGuid();
            _mockRepo.Setup(repo => repo.PlayerInGame(playerId)).Returns(true);
            _mockRepo.Setup(repo => repo.GetOpponent(playerId)).Returns(opponentId);

            // Act
            var result = _gameService.RemovePlayer(playerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(opponentId, result);
            _mockRepo.Verify(repo => repo.RemoveFromGame(playerId, opponentId), Times.Once);
        }
    }
}
