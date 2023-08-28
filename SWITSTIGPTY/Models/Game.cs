using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Models;

public class Game
{
    public string GameCode { get; set; }
    public int PlayerCount => Players.Count;
    public List<string> SongsUrls { get; set; }
    
    public List<Player> Players { get; set; }
    public int NumberOfManches { get; set; }
    public int CurrentManche { get; set; }
    public int PointPerRightVote { get; set; }
    public int PointPerVoteFooled { get; set; }
    public string? Genre { get; set; }
    public string Type { get; set; }
}