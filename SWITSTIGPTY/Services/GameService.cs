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

    public async Task<Game> CreateGame(string type, string? genre, int numberOfManches, int pointsPerRightVote = 2, int pointsPerVoteFooled = 1)
    {
        var gameCode = GenerateId();
        
        var game = new Game
        {
            GameCode = gameCode,
            SongsUrls = new List<string>(),
            Players = new List<Player>(),
            NumberOfManches = numberOfManches,
            CurrentManche = 0,
            PointPerRightVote = pointsPerRightVote,
            PointPerVoteFooled = pointsPerVoteFooled,
            Type = type,
            Genre = genre
        };

        // register the game
        _games.Add(game);
        
        return game;
    }
    
    public async Task<JoinGameDTO> JoinGame(string gameCode, string playerName)
    {
        var game = await GetGame(gameCode);
        var playerId = GenerateId();
        var player = new Player
        {
            Id = playerId,
            Name = playerName,
            VotersNames = new HashSet<string>(),
            ImageUrl = "https://www.cc-cln.fr/build/images/huchet/pictos/icon-user.png",
            IsImpostor = false,
            score = 0
        };

        game.Players.Add(player);
        
        await _gameHubService.NotifyNewPlayerNumber(gameCode, game.PlayerCount.ToString());
        
        return new JoinGameDTO
        {
            Game = game,
            Player = player
        };
    }
    
    public async Task LeaveGame(string gameCode, string playerId)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        
        if (game == null)
            throw new Exception("Game not found");
        
        game.Players.RemoveAll(p => p.Id == playerId);
        
        await _gameHubService.LeaveGroup(gameCode, playerId);
        
        await _gameHubService.NotifyNewPlayerNumber(gameCode, game.PlayerCount.ToString());
    }

    #region GenerateRandomSongsUrls
    
    private async Task<string> GetYoutubeUrl(string songName, string artistName)
    {
        var url = _randomSongApiUrl + "get-song-video?song=" + songName + "&artist="+ artistName;
        var result = await ApiUtils.GetAsync(url);
        

        return "https://youtu.be/" + result.Replace("\"", "");
    }
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
        
        await _gameHubService.NotifyGameEnded(gameCode, game.Players);
        
        
        _games.RemoveAll(g => g.GameCode == gameCode);
    }

    private async Task RandomizeAndNotifyImpostors(Game game, string emitCode, int nbImpostors = 1)
    {
        // generate songs
        var songsUrls = await GenerateSongsUrls(game.Type, game.Genre);
        game.SongsUrls = songsUrls;
        
        // select random main song number and give it to random number of players (impostors)
        var rand = new Random();
        var randomMainSongNumber = rand.Next(1);
        var otherSongNumber = randomMainSongNumber == 1 ? 0 : 1;
        
        // populate songUrl for impostors
        var impostors = game.Players.OrderBy(x => rand.Next()).Take(nbImpostors).ToList();
        impostors.ForEach(i =>
        {
            i.SongUrl = game.SongsUrls[otherSongNumber];
            i.IsImpostor = true;
        });
        
        // populate songUrl for others
        var others = game.Players.Where(p => !impostors.Contains(p)).ToList();
        others.ForEach(o => o.SongUrl = game.SongsUrls[randomMainSongNumber]);
        
        await _gameHubService.SendToGroupExceptListAsync(
            game.GameCode, 
            emitCode,
            new StartingGameDTO {Game = game, IndexOfSong = randomMainSongNumber}, 
            new StartingGameDTO {Game = game, IndexOfSong = otherSongNumber},
            impostors.Select(i => i.Id).ToList());
    }
    
    public async Task Vote(string gameCode, string votantId, string voteId)
    {
        var game = await GetGame(gameCode);
        
        var votedPlayer = game.Players.FirstOrDefault(p => p.Id == voteId);
        
        var votant = game.Players.FirstOrDefault(p => p.Id == votantId);
        
        if (votedPlayer == null || votant == null)
            throw new Exception("Player not found");

        // scoring
        if (votedPlayer.IsImpostor)
        {
            votant.score += game.PointPerRightVote;
        }
        else
        {
            var impostors = game.Players.Where(p => p.IsImpostor).ToList();
            impostors.ForEach(i => i.score += game.PointPerVoteFooled);
        }
        
        votedPlayer.VotersNames.Add(votant.Name);
        
        await _gameHubService.NotifyNewVote(gameCode, votantId);
    }

    public async Task NextRound(string gameCode, int nbImpostors = 1)
    {
        var game = await GetGame(gameCode);
        
        game.CurrentManche++;
        
        // reset players
        foreach (var gamePlayer in game.Players)
        {
            gamePlayer.VotersNames = new HashSet<string>();
            gamePlayer.SongUrl = "";
            gamePlayer.IsImpostor = false;
        }
        
        if (game.CurrentManche == game.NumberOfManches + 1)
        {
            await EndGame(gameCode);
            return;
        }
        
        await RandomizeAndNotifyImpostors(game, "next-round", nbImpostors);
    }

    public async Task EndRound(string gameCode)
    {
        var game = await GetGame(gameCode);
        
        await _gameHubService.NotifyEndRound(gameCode, game.Players);
    }
}