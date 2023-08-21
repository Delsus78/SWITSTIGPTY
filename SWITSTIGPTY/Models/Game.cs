using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Models;

public class Game : MongoBaseModel
{
    public string GameCode { get; set; }
    public int PlayerCount { get; set; }
    public List<string> SongsUrls { get; set; }
}