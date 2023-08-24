using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Models;

public class Player : MongoBaseModel
{
    public string Name { get; set; }
    public HashSet<string> Voters { get; set; }
    public string ImageUrl { get; set; }
}