namespace SWITSTIGPTY.Models;
using System.Collections.Generic;

public class Track
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<Artist> Artists { get; set; }
    public string PreviewUrl { get; set; }
    public int DurationMs { get; set; }
    public int Popularity { get; set; }
    public Album Album { get; set; }
}

public class Artist
{
    public string Name { get; set; }
    public string Id { get; set; }
}

public class Album
{
    public string Name { get; set; }
    public string AlbumType { get; set; }
    public string ReleaseDate { get; set; }
    public string Id { get; set; }
    public string ReleaseDatePrecision { get; set; }
    public string ImageDefault { get; set; }
    public string ImageLarge { get; set; }
}

public class Tracks
{
    public List<Track> tracks { get; set; }
}
