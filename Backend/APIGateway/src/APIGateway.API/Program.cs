using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var basePath = AppContext.BaseDirectory;
builder.Configuration.SetBasePath(basePath)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot();

var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();

// Replace the placeholder in the Ocelot configuration
var configuration = app.Configuration;
var downstreamHost = Environment.GetEnvironmentVariable("GAMESERVICE_HOST") ?? "localhost";
var ocelotConfig = configuration.GetSection("Routes").GetChildren().ToList();

foreach (var route in ocelotConfig)
{
    var downstreamHostAndPorts = route.GetSection("DownstreamHostAndPorts").GetChildren().ToList();
    foreach (var hostAndPort in downstreamHostAndPorts)
    {
        hostAndPort["Host"] = downstreamHost;
    }
}

configuration.GetSection("Routes").Bind(ocelotConfig);

app.UseOcelot().Wait();

app.Run();