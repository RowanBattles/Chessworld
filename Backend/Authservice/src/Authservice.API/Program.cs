using Authservice.API;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    var config = app.Services.GetRequiredService<IConfiguration>();
    var connectionString = config.GetValue<string>("ConnectionString");
    var mongoUrl = new MongoUrl(connectionString);
    var client = new MongoClient(mongoUrl);

    client.GetDatabase(mongoUrl.DatabaseName).RunCommandAsync((Command<dynamic>)"{ping:1}").Wait();
    Console.WriteLine("MongoDB connection successful.");
}
catch (Exception ex)
{
    Console.WriteLine($"MongoDB connection failed: {ex.Message}");
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
