using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SWITSTIGPTY.Models;

namespace SWITSTIGPTY.Services;

public class GameService(
    IOptions<ApiSetting> apiSetting,
    GameHubService gameHubService,
    ILogger<GameService> logger)
{
    private readonly List<Game> _games = [];
    
    private readonly string _randomSongApiUrl = apiSetting.Value.RandomSongApiUrl;

    private static string GenerateId(int length = 5)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0987654321";
        var stringChars = new char[length];
        var random = new Random();
    
        for (var i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }
    
        var finalString = new string(stringChars);
    
        return finalString;
    }

    public Game CreateGame(string type, string? genre, int numberOfManches, int pointsPerRightVote = 2, int pointsPerVoteFooled = 1)
    {
        logger.LogInformation("Creating game with type: {Type}, genre: {Genre}, numberOfManches: {NumberOfManches}, pointsPerRightVote: {PointsPerRightVote}, pointsPerVoteFooled: {PointsPerVoteFooled}", 
            type, genre, numberOfManches, pointsPerRightVote, pointsPerVoteFooled);
        
        var gameCode = GenerateId();
        
        var game = new Game
        {
            GameCode = gameCode,
            SongsUrls = [],
            Players = [],
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

    public JoinGameDTO ReconnectGame(string gameCode, string playerId)
    {
        logger.LogInformation("Reconnecting player {PlayerId} to game {GameCode}", playerId, gameCode);
        
        var game = GetGame(gameCode);
        var player = game.Players.FirstOrDefault(p => p.Id == playerId);
        
        if (player == null)
            throw new Exception("Player not found");
        
        return new JoinGameDTO {Game = game, Player = player};
    }
    
    public async Task<JoinGameDTO> JoinGame(string gameCode, string playerName)
    {
        logger.LogInformation("Joining game {GameCode} with player name {PlayerName}", gameCode, playerName);
        
        var game = GetGame(gameCode);
        var playerId = GenerateId();
        var player = new Player
        {
            Id = playerId,
            Name = playerName,
            VotersNames = [],
            ImageUrl = "https://www.cc-cln.fr/build/images/huchet/pictos/icon-user.png",
            IsImpostor = false,
            score = 0
        };

        game.Players.Add(player);
        
        await gameHubService.NotifyNewPlayerNumber(gameCode, game.PlayerCount.ToString());
        
        return new JoinGameDTO
        {
            Game = game,
            Player = player
        };
    }
    
    public async Task LeaveGame(string gameCode, string playerId)
    {
        logger.LogInformation("Leaving game {GameCode} for player {PlayerId}", gameCode, playerId);
        
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        
        if (game == null)
            throw new Exception("Game not found");
        
        game.Players.RemoveAll(p => p.Id == playerId);
        
        gameHubService.LeaveGroup(gameCode, playerId);
        
        await gameHubService.NotifyNewPlayerNumber(gameCode, game.PlayerCount.ToString());
    }

    #region GenerateRandomSongsUrls

    private async Task<List<string>> GenerateSongsUrls()
    {
        var songsUrls = await GenerateRandomSongsUrls(2);
        
        return songsUrls;
    }
    
    private Random _random = new();
    private async Task<List<string>> GenerateRandomSongsUrls(int count)
    {
        var res = new List<string>();

        var tracksForSong = await GetTracksAsync();
        
        foreach (var track in tracksForSong.results)
        {
            res.Add(track.VideoLink);
        }
        
        return res;

        async Task<Tracks> GetTracksAsync()
        {
            var body = new StringContent(JsonConvert.SerializeObject(new
            {
                numSongs = count
            }), System.Text.Encoding.UTF8, "application/json");

            var result = await ApiUtils.PostAsync(_randomSongApiUrl, body);
            return JsonConvert.DeserializeObject<Tracks>(result);
        }
    }

    #endregion

    public Game GetGame(string gameCode)
    {
        var game = _games.FirstOrDefault(g => g.GameCode == gameCode);
        if (game == null)
            throw new KeyNotFoundException($"Game with code {gameCode} not found.");
        
        return game;
    }
    
    public IEnumerable<Game> GetGames() => _games;

    public async Task EndGame(string gameCode)
    {
        var game = GetGame(gameCode);
        game.GamePhase = "end-result";
         
        await gameHubService.NotifyGameEnded(gameCode, game.Players);
        
        
        _games.RemoveAll(g => g.GameCode == gameCode);
    }

    private async Task RandomizeAndNotifyImpostors(Game game, string emitCode, int nbImpostors = 1)
    {
        // generate songs
        var songsUrls = await GenerateSongsUrls();
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
        
        await gameHubService.SendToGroupExceptListAsync(
            game.GameCode, 
            emitCode,
            new StartingGameDTO {Game = game, IndexOfSong = randomMainSongNumber}, 
            new StartingGameDTO {Game = game, IndexOfSong = otherSongNumber},
            impostors.Select(i => i.Id).ToList());
    }
    
    public async Task Vote(string gameCode, string votantId, string voteId)
    {
        logger.LogInformation("Player {VotantId} is voting for player {VoteId} in game {GameCode}", votantId, voteId, gameCode);
        
        var game = GetGame(gameCode);
        
        var votedPlayer = game.Players.FirstOrDefault(p => p.Id == voteId);
        
        var votant = game.Players.FirstOrDefault(p => p.Id == votantId);
        
        if (votedPlayer == null || votant == null)
            throw new Exception("Player not found");

        // scoring
        if (!votant.IsImpostor)
        {
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
        }
        
        await gameHubService.NotifyNewVote(gameCode, votantId);
    }

    public async Task NextRound(string gameCode, int nbImpostors = 1)
    {
        logger.LogInformation("Starting next round for game {GameCode} with {NbImpostors} impostors", gameCode, nbImpostors);
        
        var game = GetGame(gameCode);
        
        game.CurrentManche++;
        
        // reset players
        foreach (var gamePlayer in game.Players)
        {
            gamePlayer.VotersNames = [];
            gamePlayer.SongUrl = "";
            gamePlayer.IsImpostor = false;
        }
        
        if (game.CurrentManche == game.NumberOfManches + 1)
        {
            await EndGame(gameCode);
            return;
        }
        
        game.GamePhase = "started";
        
        await RandomizeAndNotifyImpostors(game, "next-round", nbImpostors);
    }

    public async Task EndRound(string gameCode)
    {
        logger.LogInformation("Ending round for game {GameCode}", gameCode);
        
        var game = GetGame(gameCode);
        game.GamePhase = "result";
        
        await gameHubService.NotifyEndRound(gameCode, game.Players);
    }
}