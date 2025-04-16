using GameService.API.Data.Repositories;
using Matchmaking.API.Business.Infrastructure;
using Matchmaking.API.Business.Services;
using Matchmaking.API.Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register services as singletons
builder.Services.AddSingleton<IMatchmakingService, MatchmakingService>();
builder.Services.AddSingleton<IMatchmakingRepository, InMemoryMatchmakingRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Disable HTTPS redirection in development
    app.Use(async (context, next) =>
    {
        context.Request.Scheme = "http";
        await next();
    });
    app.UseDeveloperExceptionPage();
}
else if (app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.MapControllers();

app.Run();
