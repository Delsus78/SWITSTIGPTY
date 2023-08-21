using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Services;

public class GameService
{
    private readonly ILogger<GameService> _logger;
    private readonly MongoDbRepository _gamesRepository;
    private List<Game> _games;
    private readonly string _randomSongApiUrl;

    public GameService(
        ILogger<GameService> logger, 
        IOptions<ConnectionSetting> connectionSetting,
        IOptions<ApiSetting> apiSetting)
    {
        _logger = logger;
        _gamesRepository = new MongoDbRepository(new MongoClient(), connectionSetting.Value.DatabaseName);
        _randomSongApiUrl = apiSetting.Value.RandomSongApiUrl;
        _games = new List<Game>();
    }
    
    private IMongoCollection<Game> GetCollection() 
        => _gamesRepository.GetCollection<Game>();
    
    public string GenerateGameCode()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var stringChars = new char[4];
        var random = new Random();
    
        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
    
        var finalString = new String(stringChars);
    
        return finalString;
    }

    public async Task<Game> CreateGame()
    {
        var gameCode = GenerateGameCode();
        
        var game = new Game
        {
            GameCode = gameCode,
            PlayerCount = 1,
            SongsUrls = new List<string>()
        };
        
        // generate songs
        var songsUrls = await GenerateSongsUrls("");
        game.SongsUrls = songsUrls;

        // register the game
        _games.Add(game);
        
        return game;
    }
    
    public async Task<Game> JoinGame(string gameCode)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        
        if (game == null)
            throw new Exception("Game not found");
        
        game.PlayerCount++;
        
        return game;
    }
    
    private async Task<List<string>> GenerateSongsUrls(string spGenree)
    {
        // recupérer la liste des mots aleatoires dans le json random_songs_words.json
        var randomSongsWords = File.ReadAllText("random_songs_words.json");
        var randomSongsWordsJson = JsonConvert.DeserializeObject<IEnumerable<string>>(randomSongsWords);
        
        if (randomSongsWordsJson == null)
            throw new Exception("random_songs_words.json is empty");
        
        var songsWordsJson = randomSongsWordsJson as string[] ?? randomSongsWordsJson.ToArray();
        
        // random Track 1
        var randomSearchWord_Song1 = songsWordsJson.ElementAt(new Random().Next(songsWordsJson.Length));
        
        var tracks_For_Song1 = await GetTracks(randomSearchWord_Song1);
        var randomTrack1 = tracks_For_Song1.tracks.ElementAt(new Random().Next(tracks_For_Song1.tracks.Count));

        var songName_Song1 = randomTrack1.Name.Replace("\"", "");
        var artistName_Song1 = randomTrack1.Artists[0].Name.Replace("\"", "");
        
        var youtubeUrl_Song1 = await GetYoutubeUrl(songName_Song1, artistName_Song1);
        
        // random Track 2
        var randomSearchWord_Song2 = songsWordsJson.ElementAt(new Random().Next(songsWordsJson.Length));
        
        var tracks_For_Song2 = await GetTracks(randomSearchWord_Song2);
        var randomTrack2 = tracks_For_Song2.tracks.ElementAt(new Random().Next(tracks_For_Song2.tracks.Count));

        var songName_Song2 = randomTrack2.Name.Replace("\"", "");
        var artistName_Song2 = randomTrack2.Artists[0].Name.Replace("\"", "");
        
        var youtubeUrl_Song2 = await GetYoutubeUrl(songName_Song2, artistName_Song2);
        
        return new List<string> {youtubeUrl_Song1, youtubeUrl_Song2};
    }

    private async Task<Tracks> GetTracks(string randomSearchWord)
    {
        var result = await ApiUtils.GetAsync(_randomSongApiUrl + "search?q=" + randomSearchWord + "&type=track&based_on=all&limit=50");
        return JsonConvert.DeserializeObject<Tracks>(result);
    }

    private async Task<string> GetYoutubeUrl(string songName, string artistName)
    {
        var url = _randomSongApiUrl + "get-song-video?song=" + songName + "&artist="+ artistName;
        var result = await ApiUtils.GetAsync(url);
        

        return "https://youtu.be/" + result.Replace("\"", "");
    }

    public async Task<Game> GetGame(string gameCode)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        if (game == null)
            throw new Exception("Game not found");
        
        return game;
    }
    
    public List<Game> GetGames()
    {
        return _games;
    }
}