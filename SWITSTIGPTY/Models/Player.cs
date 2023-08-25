using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Models;

public class Player : MongoBaseModel
{
    public string Name { get; set; }
    public HashSet<string> VotersNames { get; set; }
    public string ImageUrl { get; set; }
    public string SongUrl { get; set; }
    public bool IsImpostor { get; set; }
    public int score { get; set; }
}