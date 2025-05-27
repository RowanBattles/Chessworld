using GameService.API.API.Hubs;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Services;
using GameService.API.Data.Repository;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

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

builder.Services.AddSingleton<IGameService, GameService.API.Business.Services.GameService>();
builder.Services.AddSingleton<IGameRepository, InGameRepository>();
builder.Services.AddSingleton<IGameConnectionRepository, InGameConnectionRepository>();
builder.Services.AddSingleton<IGameConnectionService, GameConnectionService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();