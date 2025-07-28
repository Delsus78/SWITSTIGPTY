using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SWITSTIGPTY.Extensions;


public static class AppBuilderExtensions
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .ConfigureSwagger()
            .AddControllers();
        
        builder.Services
            .Configure<ApiSetting>(
                options =>
                {
                    options.RandomSongApiUrl = builder.Configuration.GetSection("ApiUrls:RandomSongApiUrl").Value;
                });
        
        builder.Services
            .ConfigureJson()
            .ConfigureCors()
            .ConfigureLogging()
            .AddApplication()
            .AddSignalR();
    }
    
    private static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddSingleton<GameService>() 
            .AddHostedService<AutoDeleteGames>()
            .AddTransient<GameHubService>();
        
    }
    
    private static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        services.AddLogging(logBuilder =>
        {
            logBuilder.ClearProviders();
            logBuilder.AddConsole();
            logBuilder.AddDebug();
            logBuilder.AddSystemdConsole();
        });
        return services;
    }
    
    private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "SWITSTIGPTY", Version = "v1" });
                c.AddSignalRSwaggerGen();
            });
        return services;
    }
    
    private static IServiceCollection ConfigureJson(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        return services;
    }
    
    private static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        return services.AddCors(o =>
        {
            o.AddPolicy("default", builder =>
            {
                Console.Out.WriteLine("Adding cors policy");
                builder.WithOrigins("http://localhost", "http://switstigpty.team-unc.fr", "https://switstigpty.team-unc.fr", "https://localhost",)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials(); // Important pour SignalR
            });
        });
    }
}
