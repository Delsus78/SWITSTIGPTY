namespace SWITSTIGPTY.Models;
using System.Collections.Generic;

public class Track
{
    public string VideoLink { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Img { get; set; }
}

public class Tracks
{
    public List<Track> results { get; set; }
}
