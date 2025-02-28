using GameService.API.Domain.DTOs;
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
        public void MatchWithFirstPersonInQueue_ShouldReturnNull_WhenQueueIsEmpty()
        {
            // Act
            var result = _repository.MatchWithFirstPersonInQueue();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void MatchWithFirstPersonInQueue_ShouldReturnPlayerId_WhenQueueIsNotEmpty()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            _repository.EnqueuePlayer(playerId);

            // Act
            var result = _repository.MatchWithFirstPersonInQueue();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(playerId, result);
        }

        [Fact]
        public void AddGame_ShouldAddGameToActiveGames()
        {
            // Arrange
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();
            var game = new Game(player1Id, player2Id);

            // Act
            _repository.AddGame(game);

            // Assert
            Assert.True(_repository.PlayerInGame(player1Id));
            Assert.True(_repository.PlayerInGame(player2Id));
            var retrievedGame = _repository.GetGameByPlayerId(player1Id);
            Assert.NotNull(retrievedGame);
            Assert.Equal(game.GameId, retrievedGame.GameId);
        }

        [Fact]
        public void RemoveGame_ShouldRemoveGameFromActiveGames()
        {
            // Arrange
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();
            var game = new Game(player1Id, player2Id);
            _repository.AddGame(game);

            // Act
            var result = _repository.RemoveGame(game.GameId);

            // Assert
            Assert.True(result);
            Assert.False(_repository.PlayerInGame(player1Id));
            Assert.False(_repository.PlayerInGame(player2Id));
        }

        [Fact]
        public void RemoveFromWaitingQueue_ShouldRemovePlayerFromQueue()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            _repository.EnqueuePlayer(playerId);

            // Act
            _repository.DequeuePlayer(playerId);

            // Assert
            var result = _repository.MatchWithFirstPersonInQueue();
            Assert.Null(result);
        }
    }
}
