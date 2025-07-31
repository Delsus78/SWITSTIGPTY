using System.Net.Http.Headers;

namespace SWITSTIGPTY;

public static class ApiUtils
{
    private static readonly HttpClient client = new();

    static ApiUtils()
    {
        // Initialisez ici si vous voulez définir des en-têtes par défaut, des délais d'attente, etc.
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Referer", "https://www.chosic.com/random-songs-generator-with-links-to-spotify-and-youtube/");
    }

    public static async Task<string> GetAsync(string uri)
    {
        var response = await client.GetAsync(uri);
        response.EnsureSuccessStatusCode(); // Lève une exception si la réponse n'est pas réussie.
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> PostAsync(string uri, HttpContent content)
    {
        var response = await client.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<bool> IsUrlValidAsync(string uri)
    {
        try
        {
            var response = await client.GetAsync(uri);
            var html = await response.Content.ReadAsStringAsync();
            return !html.Contains("UNPLAYABLE") && html.Contains("og:image");
        }
        catch
        {
            return false; // Si une exception est levée, on considère que l'URL n'est pas valide.
        }
    }
}