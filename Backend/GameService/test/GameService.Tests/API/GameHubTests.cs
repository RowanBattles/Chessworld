using GameService.API.API.Hubs;
using GameService.API.Business.Services;
using GameService.API.src.Business.Services;
using GameService.API.src.Domain.DTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GameService.Tests.API
{
    public class GameHubTests
    {
        private readonly IHost _host;
        private readonly HubConnection _connection;
        private readonly Mock<IGameService> _mockGameService;
        private readonly Mock<IPlayerService> _mockPlayerService;

        public GameHubTests()
        {
            _mockGameService = new Mock<IGameService>();
            _mockPlayerService = new Mock<IPlayerService>();

            _host = new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseTestServer();
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddSignalR();
                        services.AddSingleton(_mockGameService.Object);
                        services.AddSingleton(_mockPlayerService.Object);
                    });
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapHub<GameHub>("/gamehub");
                        });
                    });
                })
                .Start();

            var server = _host.GetTestServer();
            _connection = new HubConnectionBuilder()
                .WithUrl(server.BaseAddress + "gamehub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => server.CreateHandler();
                })
                .Build();

            _connection.StartAsync().Wait();
        }

        [Fact]
        public async Task ConnectToHub_ShouldSucceed()
        {
            // Assert
            Assert.Equal(HubConnectionState.Connected, _connection.State);

            // Cleanup
            await _connection.StopAsync();
        }

        [Fact]
        public async Task FindGame_ShouldReturnWaiting_WhenNoMatchFound()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            _mockGameService.Setup(service => service.MatchPlayer(playerId)).Returns((FindGameResponseDto?)null);

            // Act
            var result = await _connection.InvokeAsync<FindGameResponseDto>("FindGame", new FindGameRequestDto(playerId));

            // Assert
            Assert.Null(result);

            // Cleanup
            await _connection.StopAsync();
        }

        [Fact]
        public async Task FindGame_ShouldReturnOpponent_WhenMatchFound()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var opponentId = Guid.NewGuid();
            var response = new FindGameResponseDto(opponentId);
            _mockGameService.Setup(service => service.MatchPlayer(playerId)).Returns(response);
            _mockPlayerService.Setup(service => service.GetConnectionId(opponentId)).Returns("opponent-connection-id");

            FindGameResponseDto? receivedResponse = null;
            _connection.On<Guid>("GameFound", opponentId =>
            {
                receivedResponse = new FindGameResponseDto(opponentId);
            });

            // Act
            await _connection.InvokeAsync("FindGame", new FindGameRequestDto(playerId));

            // Assert
            Assert.NotNull(receivedResponse);
            Assert.Equal(opponentId, receivedResponse.OpponentId);

            // Cleanup
            await _connection.StopAsync();
        }

        [Fact]
        public async Task LeaveGame_ShouldNotifyOpponent_WhenInGame()
        {
            // Arrange
            var playerId1 = Guid.NewGuid();
            var playerId2 = Guid.NewGuid();
            _mockGameService.Setup(service => service.MatchPlayer(playerId1)).Returns(new FindGameResponseDto(playerId2));
            _mockGameService.Setup(service => service.MatchPlayer(playerId2)).Returns(new FindGameResponseDto(playerId1));
            _mockGameService.Setup(service => service.RemovePlayer(playerId1)).Returns(playerId2);
            _mockPlayerService.Setup(service => service.GetConnectionId(playerId2)).Returns("opponent-connection-id");

            var mockClients = new Mock<IHubCallerClients>();
            var mockCaller = new Mock<ISingleClientProxy>();
            var mockOpponent = new Mock<ISingleClientProxy>();

            mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);
            mockClients.Setup(clients => clients.Client("opponent-connection-id")).Returns(mockOpponent.Object);

            var hubContext = new Mock<HubCallerContext>();
            hubContext.Setup(context => context.ConnectionId).Returns("player1-connection-id");

            var gameHub = new GameHub(_mockGameService.Object, _mockPlayerService.Object)
            {
                Clients = mockClients.Object,
                Context = hubContext.Object
            };

            await gameHub.FindGame(new FindGameRequestDto(playerId1));
            await gameHub.FindGame(new FindGameRequestDto(playerId2));

            // Act
            await gameHub.LeaveGame(new FindGameRequestDto(playerId1));

            // Assert
            mockCaller.Verify(caller => caller.SendCoreAsync("GameLeft", new object[] { "You have left the game" }, It.IsAny<CancellationToken>()), Times.Once);
            mockOpponent.Verify(opponent => opponent.SendCoreAsync("GameLeft", new object[] { "Opponent has left the game" }, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
