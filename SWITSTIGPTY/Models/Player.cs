using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Models;

public class Player : MongoBaseModel
{
    public string Name { get; set; }
    public string id { get; set; }
}