using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Models;

public class Game
{
    public string GameCode { get; set; }
    public int PlayerCount => Players.Count;
    public List<string> SongsUrls { get; set; }
    
    public List<Player> Players { get; set; }
}