using GameService.API.API.Hubs;
using GameService.API.Business.Services;
using GameService.API.Data.Repositories;
using GameService.API.src.Business.Services;
using GameService.API.src.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
    });
});


builder.Services.AddSingleton<IGameService, GameService.API.Business.Services.GameService>();
builder.Services.AddSingleton<IGameRepository, InGameMemoryRepository>();
builder.Services.AddSingleton<IPlayerService, PlayerService>();
builder.Services.AddSingleton<IPlayerRepository, PlayerMemoryRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.MapHub<GameHub>("/gamehub");

app.Run();