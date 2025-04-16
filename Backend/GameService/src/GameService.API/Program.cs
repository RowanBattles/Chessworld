using GameService.API.API.Hubs;
using GameService.API.Business.Interfaces;
using GameService.API.Business.Services;
using GameService.API.Data.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowCredentials();
    });
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

app.MapHub<GameHub>("/play");
app.MapHub<GameHub>("/watch");

app.Run();