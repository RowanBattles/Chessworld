using GameService.API.API.Hubs;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Services;
using GameService.API.Data.Repository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Error);


builder.Services.AddSignalR();

builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
    if (string.IsNullOrEmpty(redisConnectionString))
    {
        throw new InvalidOperationException("Redis connection string is not configured.");
    }

    var redisConfiguration = ConfigurationOptions.Parse(redisConnectionString, true);
    return ConnectionMultiplexer.Connect(redisConfiguration);
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxConcurrentConnections = 20000; // Increase if needed
    serverOptions.Limits.MaxConcurrentUpgradedConnections = 20000;
    serverOptions.Limits.MaxRequestBufferSize = 1048576; // 1MB, adjust as needed
    serverOptions.Limits.MaxRequestLineSize = 8192; // 8KB, adjust as needed
    serverOptions.Limits.MaxRequestHeadersTotalSize = 32768; // 32KB, adjust as needed
    serverOptions.ListenAnyIP(8080); // Explicitly bind to 8080 if not already
});

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string is not configured."), name: "redis");



builder.Services.AddSingleton<IGameService, GameService.API.Business.Services.GameService>();
builder.Services.AddSingleton<IGameRepository, InGameRepository>();
builder.Services.AddSingleton<IGameConnectionRepository, InGameConnectionRepository>();
builder.Services.AddSingleton<IGameConnectionService, GameConnectionService>();
builder.Services.AddScoped<InGameConnectionRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<RedisGameMoveListener>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.UseWebSockets();

app.MapHub<GameHub>("/play");
app.MapHub<GameHub>("/watch");

var redis = app.Services.GetRequiredService<IConnectionMultiplexer>();
try
{
    var db = redis.GetDatabase();
    var pong = await db.PingAsync();
    Console.WriteLine($"Redis is running. Ping: {pong.TotalMilliseconds} ms");
}
catch (Exception ex)
{
    Console.WriteLine($"Redis connection failed: {ex.Message}");
    throw;
}

app.Run();