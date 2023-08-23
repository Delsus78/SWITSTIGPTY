using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5000");

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

// cors
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        {
            Console.Out.WriteLine("Adding cors policy");
            builder.WithOrigins("http://localhost:5173", "http://switstigpty.team-unc.fr") // Ajoutez votre domaine client ici
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials(); // Important pour SignalR
        });
});


// services
services.AddSingleton<GameService>();
services.AddHostedService<AutoDeleteGames>();
services.AddTransient<GameHubService>();

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
app.UseCors();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/hubs/GameHub");


app.Run();