using GameService.API.Data.Repository;
using GameService.API.Business.Models;
using GameService.API.Data.Entity;
using GameService.API.Contract.Mappers;
using Xunit;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GameService.API.Contract.Enums;

namespace GameService.Tests.Data.Repository
{
    public class InGameRepositoryTests
    {
        private readonly InGameRepository _repository;

        public InGameRepositoryTests()
        {
            _repository = new InGameRepository();
        }

        [Fact]
        public async Task AddGame_ShouldAddGameToRepository()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken");

            // Act
            await _repository.AddGame(gameModel);

            // Assert
            var addedGame = await _repository.GetGameByGameId(gameModel.Id);
            Assert.NotNull(addedGame);
            Assert.Equal(gameModel.Id, addedGame!.Id);
            Assert.Equal(gameModel.WhiteToken, addedGame.WhiteToken);
            Assert.Equal(gameModel.BlackToken, addedGame.BlackToken);
        }

        [Fact]
        public async Task GetGameByGameId_ShouldReturnGame_WhenGameExists()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken");
            await _repository.AddGame(gameModel);

            // Act
            var result = await _repository.GetGameByGameId(gameModel.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(gameModel.Id, result!.Id);
        }

        [Fact]
        public async Task GetGameByGameId_ShouldReturnNull_WhenGameDoesNotExist()
        {
            // Act
            var result = await _repository.GetGameByGameId(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGameByPlayerId_ShouldReturnGameId_WhenPlayerTokenExists()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken");
            await _repository.AddGame(gameModel);

            // Act
            var result = await _repository.GetGameByPlayerId("whiteToken");

            // Assert
            Assert.Equal(gameModel.Id, result);
        }

        [Fact]
        public async Task GetGameByPlayerId_ShouldReturnEmptyGuid_WhenPlayerTokenDoesNotExist()
        {
            // Act
            var result = await _repository.GetGameByPlayerId("nonExistentToken");

            // Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public async Task UpdateGame_ShouldUpdateGame_WhenGameExists()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken", GameStatus.InProgress, "initialFen");
            await _repository.AddGame(gameModel);

            var updatedGameModel = new GameModel(gameModel.Id, "whiteToken", "blackToken", GameStatus.Finished, "updatedFen");

            // Act
            await _repository.UpdateGame(updatedGameModel);

            // Assert
            var updatedGame = await _repository.GetGameByGameId(gameModel.Id);
            Assert.NotNull(updatedGame);
            Assert.Equal(GameStatus.Finished, updatedGame!.Status);
            Assert.Equal("updatedFen", updatedGame.Fen);
        }

        [Fact]
        public async Task UpdateGame_ShouldThrowKeyNotFoundException_WhenGameDoesNotExist()
        {
            // Arrange
            var gameModel = new GameModel(Guid.NewGuid(), "whiteToken", "blackToken");

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.UpdateGame(gameModel));
        }
    }
}
