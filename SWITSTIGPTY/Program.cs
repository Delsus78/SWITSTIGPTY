using SWITSTIGPTY.Controllers;
using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddControllers();

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SWITSTIGPTY", Version = "v1" });
    c.AddSignalRSwaggerGen();
});

services.Configure<ConnectionSetting>(
    options =>
    {
        options.ConnectionString = configuration.GetSection("MongoDb:ConnectionString").Value;
        options.DatabaseName = configuration.GetSection("MongoDb:DatabaseName").Value;
    });

services.Configure<ApiSetting>(
    options =>
    {
        options.RandomSongApiUrl = configuration.GetSection("ApiUrls:RandomSongApiUrl").Value;
    });

// services
services.AddSingleton<GameService>();

// Hub
services.AddSignalR();

// logger
services.AddLogging(logBuilder =>
{
    logBuilder.ClearProviders();
    logBuilder.AddConsole();
    logBuilder.AddDebug();
    logBuilder.AddSystemdConsole();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/hub/game");

app.Run();