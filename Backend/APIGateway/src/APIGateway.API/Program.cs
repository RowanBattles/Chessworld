using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Optional: Remove this if credentials are not needed
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);

var basePath = AppContext.BaseDirectory;
builder.Configuration.SetBasePath(basePath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var ocelotJsonPath = Path.Combine(basePath, "ocelot.json");
if (File.Exists(ocelotJsonPath))
{
    var ocelotJsonContent = File.ReadAllText(ocelotJsonPath);

    // Replace placeholders with environment variable values
    var matchmakingServiceHost = Environment.GetEnvironmentVariable("MatchmakingServiceHost") ?? (env == "Docker" ? "matchmakingservice" : "localhost");
    var gameServiceHost = Environment.GetEnvironmentVariable("GameServiceHost") ?? (env == "Docker" ? "gameservice" : "localhost");

    ocelotJsonContent = ocelotJsonContent
        .Replace("{MatchmakingServiceHost}", matchmakingServiceHost)
        .Replace("{GameServiceHost}", gameServiceHost);

    //Console.WriteLine("Contents of ocelot.json after replacements:");
    //Console.WriteLine(ocelotJsonContent);

    // Load the modified JSON content into the configuration
    using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(ocelotJsonContent));
    builder.Configuration.AddJsonStream(stream);
}
else
{
    Console.WriteLine("ocelot.json file not found.");
}

var app = builder.Build();

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment() || env == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Use(async (context, next) =>
    {
        context.Request.Scheme = "http";
        await next();
    });
    app.UseDeveloperExceptionPage();
}

app.UseAuthorization();

app.UseWebSockets();

await app.UseOcelot();

app.Run();