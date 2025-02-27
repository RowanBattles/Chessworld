using GameService.API.src.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Tests.Data
{
    public class InGameMemoryRepositoryTests
    {
        private readonly InGameMemoryRepository _repository;

        public InGameMemoryRepositoryTests()
        {
            _repository = new InGameMemoryRepository();
        }

        [Fact]
        public void TryDequeueOpponent_ShouldReturnFalse_WhenQueueIsEmpty()
        {
            // Act
            var result = _repository.TryDequeueOpponent(out Guid opponentId);

            // Assert
            Assert.False(result);
            Assert.Equal(Guid.Empty, opponentId);
        }

        [Fact]
        public void TryDequeueOpponent_ShouldReturnTrue_WhenQueueIsNotEmpty()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            _repository.EnqueuePlayer(playerId);

            // Act
            var result = _repository.TryDequeueOpponent(out Guid opponentId);

            // Assert
            Assert.True(result);
            Assert.Equal(playerId, opponentId);
        }

        [Fact]
        public void AddToActiveGames_ShouldAddPlayersToActiveGames()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var opponentId = Guid.NewGuid();

            // Act
            _repository.AddToActiveGames(playerId, opponentId);

            // Assert
            Assert.True(_repository.PlayerInGame(playerId));
            Assert.True(_repository.PlayerInGame(opponentId));
            Assert.Equal(opponentId, _repository.GetOpponent(playerId));
            Assert.Equal(playerId, _repository.GetOpponent(opponentId));
        }

        [Fact]
        public void RemoveFromGame_ShouldRemovePlayersFromActiveGames()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var opponentId = Guid.NewGuid();
            _repository.AddToActiveGames(playerId, opponentId);

            // Act
            _repository.RemoveFromGame(playerId, opponentId);

            // Assert
            Assert.False(_repository.PlayerInGame(playerId));
            Assert.False(_repository.PlayerInGame(opponentId));
        }

        [Fact]
        public void RemoveFromWaitingQueue_ShouldRemovePlayerFromQueue()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            _repository.EnqueuePlayer(playerId);

            // Act
            _repository.RemoveFromWaitingQueue(playerId);

            // Assert
            var result = _repository.TryDequeueOpponent(out Guid opponentId);
            Assert.False(result);
        }
    }
}
