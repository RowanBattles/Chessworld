//using GameService.API.API.Hubs;
//using GameService.API.Business.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.Connections.Features;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.Extensions.Logging;
//using Moq;
//using Xunit;

//public class GameHubTests
//{
//    private readonly Mock<IGameConnectionService> _mockGameConnectionService;
//    private readonly Mock<IGameService> _mockGameService;
//    private readonly Mock<ILogger<GameHub>> _mockLogger;
//    private readonly Mock<HubCallerContext> _mockContext;
//    private readonly Mock<IClientProxy> _mockClientProxy;
//    private readonly Mock<IHubCallerClients> _mockClients;
//    private readonly Mock<IGroupManager> _mockGroups;
//    private readonly GameHub _gameHub;

//    public GameHubTests()
//    {
//        _mockGameConnectionService = new Mock<IGameConnectionService>();
//        _mockGameService = new Mock<IGameService>();
//        _mockLogger = new Mock<ILogger<GameHub>>();
//        _mockContext = new Mock<HubCallerContext>();
//        _mockClientProxy = new Mock<IClientProxy>();
//        _mockClients = new Mock<IHubCallerClients>();
//        _mockGroups = new Mock<IGroupManager>();

//        _gameHub = new GameHub(
//            _mockGameConnectionService.Object,
//            _mockGameService.Object,
//            _mockLogger.Object)
//        {
//            Context = _mockContext.Object,
//            Clients = _mockClients.Object,
//            Groups = _mockGroups.Object
//        };
//    }

//    [Fact]
//    public async Task OnConnectedAsync_ShouldAbort_WhenHttpContextIsNull()
//    {
//        // Arrange
//        _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
//        _mockContext.Setup(c => c.Items).Returns(new Dictionary<object, object?>());
//        _mockContext.Setup(c => c.Features.Get<IHttpContextFeature>()).Returns((IHttpContextFeature?)null);

//        // Act
//        await _gameHub.OnConnectedAsync();

//        // Assert
//        _mockContext.Verify(c => c.Abort(), Times.Once);
//    }

//    [Fact]
//    public async Task OnConnectedAsync_ShouldAbort_WhenGameIdIsInvalid()
//    {
//        // Arrange
//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Query = new QueryCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
//        {
//            { "gameId", "invalid-guid" }
//        });

//        var httpContextFeature = new Mock<IHttpContextFeature>();
//        httpContextFeature.Setup(c => c.HttpContext).Returns(httpContext);

//        var featureCollection = new FeatureCollection();
//        featureCollection.Set<IHttpContextFeature>(httpContextFeature.Object);

//        _mockContext.Setup(c => c.Features).Returns(featureCollection);
//        _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");

//        // Act
//        await _gameHub.OnConnectedAsync();

//        // Assert
//        _mockContext.Verify(c => c.Abort(), Times.Once);
//    }

//    [Fact]
//    public async Task OnDisconnectedAsync_ShouldLogInformation()
//    {
//        // Arrange
//        _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
//        _ = _mockLogger.Setup(
//            l => l.Log(
//                LogLevel.Information,
//                It.IsAny<EventId>(),
//                It.Is<It.IsAnyType>((c, v) => c.ToString()!.Contains("Connection test-connection-id disconnected.")),
//                It.IsAny<Exception>(),
//                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
//            )
//        );

//        // Act
//        await _gameHub.OnDisconnectedAsync(null);

//        // Assert
//        _mockLogger.Verify(
//            l => l.Log(
//                LogLevel.Information,
//                It.IsAny<EventId>(),
//                It.Is<It.IsAnyType>((c, v) => c.ToString()!.Contains("Connection test-connection-id disconnected.")),
//                It.IsAny<Exception>(),
//                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
//            ),
//            Times.Once
//        );
//    }

//    [Fact]
//    public async Task MakeMove_ShouldAbort_WhenConnectionIdIsNull()
//    {
//        // Arrange
//        _mockContext.Setup(c => c.ConnectionId).Returns(string.Empty);

//        // Act
//        await _gameHub.MakeMove("e2e4");

//        // Assert
//        _mockContext.Verify(c => c.Abort(), Times.Once);
//    }

//    [Fact]
//    public async Task MakeMove_ShouldSendError_WhenColorIsNull()
//    {
//        // Arrange
//        _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
//        _mockGameConnectionService
//            .Setup(s => s.GetGameIdByConnectionId(It.IsAny<string>()))
//            .ReturnsAsync(Guid.NewGuid());
//        _mockGameConnectionService
//            .Setup(s => s.GetColorByConnectionId(It.IsAny<Guid>(), It.IsAny<string>()))
//            .ReturnsAsync((string?)null);

//        var mockSingleClientProxy = new Mock<ISingleClientProxy>();
//        _mockClients.Setup(c => c.Caller).Returns(mockSingleClientProxy.Object);

//        // Act
//        await _gameHub.MakeMove("e2e4");

//        // Assert
//        mockSingleClientProxy.Verify(
//            c => c.SendCoreAsync("Error", It.Is<object[]>(o => o[0].ToString() == "You are not assigned a color in this game."), default),
//            Times.Once);
//    }


//    [Fact]
//    public async Task MakeMove_ShouldSendReceiveMove_WhenMoveIsValid()
//    {
//        // Arrange
//        var gameId = Guid.NewGuid();
//        _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
//        _mockGameConnectionService
//            .Setup(s => s.GetGameIdByConnectionId(It.IsAny<string>()))
//            .ReturnsAsync(gameId);
//        _mockGameConnectionService
//            .Setup(s => s.GetColorByConnectionId(It.IsAny<Guid>(), It.IsAny<string>()))
//            .ReturnsAsync("white");
//        _mockGameService
//            .Setup(s => s.MakeMove(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
//            .ReturnsAsync("new-fen");
//        _mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);

//        // Act
//        await _gameHub.MakeMove("e2e4");

//        // Assert
//        _mockClientProxy.Verify(
//            c => c.SendCoreAsync("ReceiveMove", It.Is<object[]>(o => o[0].ToString() == "new-fen"), default),
//            Times.Once);
//    }
//}
