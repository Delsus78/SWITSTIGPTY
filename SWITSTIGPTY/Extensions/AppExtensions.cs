using System.Net;
using SWITSTIGPTY.Services;

namespace SWITSTIGPTY.Extensions;

public static class AppExtensions
{
    public static IApplicationBuilder ConfigureApp(this WebApplication app)
    {
        app.UseCors("default");
        app.ConfigureSwagger();

        app.UseStaticFiles(); // Sert les fichiers statiques du frontend

        app.UseHttpsRedirection()
            .UseAuthorization();
        
        app.MapControllers(); 
        app.MapHub<GameHub>("/hubs/GameHub");

        // Fallback SPA : renvoie index.html pour toute route non-API/non statique
        app.MapFallbackToFile("index.html");
        
        return app;
    }
    
    private static void ConfigureSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cassiopée V0");
            c.OAuthClientId("cassiopee");
            c.OAuthAppName("Demo API - Swagger");
        });

        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync($"Une erreur s'est produite : {ex.Message}");
            }
        });
    }
}