using GameService.API.src.Domain.DTOs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameService.Tests.API
{
    public class GameHubTests
    {
        private readonly HubConnection _connection;

        public GameHubTests()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/gamehub")
                .Build();
        }
        [Fact]
        public async Task ConnectToHub_ShouldSucceed()
        {
            // Act
            await _connection.StartAsync();

            // Assert
            Assert.Equal(HubConnectionState.Connected, _connection.State);

            // Cleanup
            await _connection.StopAsync();
        }

        [Fact]
        public async Task MatchPlayer_ShouldReturnWaiting_WhenNoMatchFound()
        {
            // Arrange
            await _connection.StartAsync();
            var playerId = Guid.NewGuid();

            // Act
            var result = await _connection.InvokeAsync<FindGameResponseDto>("MatchPlayer", playerId);

            // Assert
            Assert.Equal(playerId, result.OpponentId);

            // Cleanup
            await _connection.StopAsync();
        }

        [Fact]
        public async Task MatchPlayer_ShouldReturnOpponent_WhenMatchFound()
        {
            // Arrange
            await _connection.StartAsync();
            var playerId1 = Guid.NewGuid();
            var playerId2 = Guid.NewGuid();

            // Act
            var result1 = await _connection.InvokeAsync<FindGameResponseDto>("MatchPlayer", playerId1);
            var result2 = await _connection.InvokeAsync<FindGameResponseDto>("MatchPlayer", playerId2);

            // Assert
            Assert.Equal(playerId1, result2.OpponentId);

            // Cleanup
            await _connection.StopAsync();
        }

        [Fact]
        public async Task RemovePlayer_ShouldNotifyOpponent_WhenInGame()
        {
            // Arrange
            await _connection.StartAsync();
            var playerId1 = Guid.NewGuid();
            var playerId2 = Guid.NewGuid();

            await _connection.InvokeAsync<FindGameResponseDto>("MatchPlayer", playerId1);
            await _connection.InvokeAsync<FindGameResponseDto>("MatchPlayer", playerId2);

            // Act
            var result = await _connection.InvokeAsync<Guid?>("RemovePlayer", playerId1);

            // Assert
            Assert.Equal(playerId2, result);

            // Cleanup
            await _connection.StopAsync();
        }
    }
}
