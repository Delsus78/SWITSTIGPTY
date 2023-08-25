using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services.Repositories;

namespace SWITSTIGPTY.Services;

public class GameService
{
    private readonly ILogger<GameService> _logger;
    private readonly MongoDbRepository _gamesRepository;
    
    private readonly List<Game> _games;
    
    private readonly string _randomSongApiUrl;
    private readonly GameHubService _gameHubService;
    private readonly List<string> _all_genres;
    private readonly List<string> _all_random_songs_words;

    public GameService(
        ILogger<GameService> logger, 
        IOptions<ConnectionSetting> connectionSetting,
        IOptions<ApiSetting> apiSetting,
        GameHubService gameHubService)
    {
        _logger = logger;
        _gamesRepository = new MongoDbRepository(new MongoClient(), connectionSetting.Value.DatabaseName);
        _randomSongApiUrl = apiSetting.Value.RandomSongApiUrl;
        _games = new List<Game>();
        _gameHubService = gameHubService;
        _all_genres = GetJsonOfAllGenres();
        _all_random_songs_words = GetJsonOfRandomSongsWords();
    }

    public string GenerateId(int length = 5)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321";
        var stringChars = new char[length];
        var random = new Random();
    
        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
    
        var finalString = new String(stringChars);
    
        return finalString;
    }

    public async Task<Game> CreateGame(string type, string? genre)
    {
        var gameCode = GenerateId();
        
        var game = new Game
        {
            GameCode = gameCode,
            SongsUrls = new List<string>(),
            Players = new List<Player>()
        };
        
        // generate songs
        var songsUrls = await GenerateSongsUrls(type, genre);
        game.SongsUrls = songsUrls;

        // register the game
        _games.Add(game);
        
        return game;
    }
    
    public async Task<JoinGameDTO> JoinGame(string gameCode, string playerName)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        var playerId = GenerateId();
        
        if (game == null)
            throw new Exception("Game not found");

        
        game.Players.Add(new Player
        {
            Id = playerId,
            Name = playerName,
            VotersNames = new HashSet<string>(),
            ImageUrl = "https://www.cc-cln.fr/build/images/huchet/pictos/icon-user.png"
        });
        
        await _gameHubService.NotifyNewPlayerNumber(gameCode, game.PlayerCount.ToString());
        
        return new JoinGameDTO
        {
            GameCode = gameCode,
            SongsUrls = game.SongsUrls,
            PlayerCount = game.PlayerCount,
            PlayerId = playerId
        };
    }
    
    public async Task LeaveGame(string gameCode, string playerId)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        
        if (game == null)
            throw new Exception("Game not found");
        
        game.Players.RemoveAll(p => p.Id == playerId);

        await _gameHubService.NotifyNewPlayerNumber(gameCode, game.PlayerCount.ToString());
    }

    #region GenerateRandomSongsUrls
    
    private List<string> GetJsonOfAllGenres()
    {
        var allgenres = File.ReadAllText("all_genres.json");
        
        if (allgenres == null)
            throw new Exception("all_genres.json is empty");
        
        return JsonConvert.DeserializeObject<List<string>>(allgenres);
    }
    
    private List<string> GetJsonOfRandomSongsWords()
    {
        var randomSongsWords = File.ReadAllText("random_songs_words.json");
        
        if (randomSongsWords == null)
            throw new Exception("random_songs_words.json is empty");
        
        return JsonConvert.DeserializeObject<List<string>>(randomSongsWords);
    }
    
    public IEnumerable<string> GetAllGenres() => _all_genres;
    public IEnumerable<string> GetRandomSongsWords() => _all_random_songs_words;

    private async Task<List<string>> GenerateSongsUrls(string type, string? spGenree)
    {
        // check types
        type = type.ToLower();
        spGenree = spGenree?.ToLower();

        var songsUrls = new List<string>();
        switch (type)
        {
            case "all":
                songsUrls = await GenerateRandomAllSongsUrls(2);
                break;
            case "top-all-time":
                songsUrls = await GenerateRandomTypedSongsUrls("top-all-time", 2);
                break;
            case "genre":
                songsUrls = await GenerateRandomGenredSongsUrls(spGenree, 2);
                break;
            default:
                throw new Exception("Type is not correct");
        }
        
        // generate songs urls
        
        
        return songsUrls;
    }
    
    private Random _random = new();
    private async Task<List<string>> GenerateRandomSongsUrls(string keyword, int count, TrackSource trackSource)
    {
        var res = new List<string>();

        async Task<Tracks> GetTracksAsync()
        {
            string apiUrl = trackSource switch
            {
                TrackSource.Type => $"{_randomSongApiUrl}random-songs?key={keyword}&limit=51",
                TrackSource.Genre => $"{_randomSongApiUrl}search?q={keyword}&type=&based_on=genre",
                TrackSource.RandomWord => $"{_randomSongApiUrl}search?q={keyword}&type=track&based_on=all&limit=50",
                _ => throw new InvalidOperationException("Unknown track source")
            };

            var result = await ApiUtils.GetAsync(apiUrl);
            return JsonConvert.DeserializeObject<Tracks>(result);
        }

        for (var i = 0; i < count; i++)
        {
            var tracksForSong = await GetTracksAsync();
            var randomTrack = tracksForSong.tracks.ElementAt(_random.Next(tracksForSong.tracks.Count));

            var songNameSong = randomTrack.Name.Replace("\"", "");
            var artistNameSong = randomTrack.Artists[0].Name.Replace("\"", "");

            var youtubeUrlSong = await GetYoutubeUrl(songNameSong, artistNameSong);

            res.Add(youtubeUrlSong);
        }

        return res;
    }

    public async Task<List<string>> GenerateRandomTypedSongsUrls(string type, int count)
        => await GenerateRandomSongsUrls(type, count, TrackSource.Type);

    public async Task<List<string>> GenerateRandomGenredSongsUrls(string genre, int count)
        => await GenerateRandomSongsUrls(genre, count, TrackSource.Genre);

    public async Task<List<string>> GenerateRandomAllSongsUrls(int count)
    {
        var songsWords = GetRandomSongsWords().ToList();
        var randomSearchWordSong = songsWords.ElementAt(_random.Next(songsWords.Count));

        return await GenerateRandomSongsUrls(randomSearchWordSong, count, TrackSource.RandomWord);
    }

    enum TrackSource
    {
        Type,
        Genre,
        RandomWord
    }
    #endregion
    
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
    
    public IEnumerable<Game> GetGames() => _games;

    public async Task EndGame(string gameCode)
    {
        var game = await GetGame(gameCode);
        
        _gameHubService.NotifyGameEnded(gameCode, game.Players);
        
        
        _games.RemoveAll(g => g.GameCode == gameCode);
    }

    public async Task StartGame(string gameCode, int nbImpostors = 1)
    {
        const string emitName = "start-game";
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        if (game == null)
            throw new Exception("Game not found");
        
        if (game.PlayerCount < nbImpostors + 1)
            throw new Exception("Not enough players");

        // select random main song number and give it to random number of players (impostors)
        var rand = new Random();
        var randomMainSongNumber = rand.Next(1);
        var otherSongNumber = randomMainSongNumber == 1 ? 0 : 1;
        
        // populate songUrl for impostors
        var impostors = game.Players.OrderBy(x => rand.Next()).Take(nbImpostors).ToList();
        impostors.ForEach(i => i.SongUrl = game.SongsUrls[otherSongNumber]);
        
        // populate songUrl for others
        var others = game.Players.Where(p => !impostors.Contains(p)).ToList();
        others.ForEach(o => o.SongUrl = game.SongsUrls[randomMainSongNumber]);
        
        await _gameHubService.SendToGroupExceptListAsync(
            gameCode, 
            emitName,
            new StartingGameDTO {Players = game.Players, IndexOfSong = randomMainSongNumber}, 
            new StartingGameDTO {Players = game.Players, IndexOfSong = otherSongNumber},
            impostors.Select(i => i.Id).ToList());
    }

    public async Task Vote(string gameCode, string votantId, string voteId)
    {
        var game = await GetGame(gameCode);
        
        var player = game.Players.FirstOrDefault(p => p.Id == voteId);
        
        var votant = game.Players.FirstOrDefault(p => p.Id == votantId);
        
        if (player == null || votant == null)
            throw new Exception("Player not found");

        player.VotersNames.Add(votant.Name);
        
        await _gameHubService.NotifyNewVote(gameCode, votantId);
    }
}