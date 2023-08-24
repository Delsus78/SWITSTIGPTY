namespace SWITSTIGPTY.Models;

public class JoinGameDTO
{
    public string GameCode { get; set; }
    public int PlayerCount { get; set; }
    public List<string> SongsUrls { get; set; }
    public string PlayerId { get; set; }
}