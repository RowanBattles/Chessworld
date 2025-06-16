using GameService.API.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Threading;
using System.Threading.Tasks;

public class RedisGameMoveListener : IHostedService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IHubContext<GameHub> _hubContext;

    public RedisGameMoveListener(IConnectionMultiplexer redis, IHubContext<GameHub> hubContext)
    {
        _redis = redis;
        _hubContext = hubContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var subscriber = _redis.GetSubscriber();
        subscriber.Subscribe(RedisChannel.Pattern("game:*"), async (channel, message) =>
        {
            var gameId = channel.ToString().Split(':')[1];
            Console.WriteLine($"[RedisGameMoveListener] Broadcasting move for game {gameId}: {message}");
            await _hubContext.Clients.Group(gameId).SendAsync("ReceiveMove", message.ToString());
        });
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
