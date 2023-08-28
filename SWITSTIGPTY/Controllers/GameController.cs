using Microsoft.AspNetCore.Mvc;
using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services;

namespace SWITSTIGPTY.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{

    private readonly ILogger<GameController> _logger;
    private readonly GameService _gameService;

    public GameController(
        ILogger<GameController> logger,
        GameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">
    /// top-all-time
    /// all
    /// genre</param>
    /// <param name="genre">
    /// </param>
    /// <returns></returns>
    [HttpGet(Name = "CreateGame")]
    public async Task<Game> CreateGame(string type, string? genre, int numberOfManches, int pointsPerRightVote, int pointsPerVoteFooled)
    {
        return await _gameService.CreateGame(type, genre, numberOfManches, pointsPerRightVote, pointsPerVoteFooled);
    }
    
    [HttpGet("{gameCode}", Name = "GetGame")]
    public async Task<Game> GetGame(string gameCode)
    {
        return await _gameService.GetGame(gameCode);
    }

    [HttpGet("all", Name = "GetGames")]
    public async Task<IEnumerable<Game>> GetGames()
    {
        return _gameService.GetGames();
    }
    
    [HttpGet("allgenres", Name = "GetAllGenres")]
    public async Task<IEnumerable<string>> GetAllGenres()
    {
        return _gameService.GetAllGenres();
    }
    
    [HttpPost("{gameCode}/join", Name = "JoinGame")]
    public async Task<JoinGameDTO> JoinGame(string gameCode, string playerName)
    {
        return await _gameService.JoinGame(gameCode, playerName);
    }
    
    [HttpPost("{gameCode}/leave", Name = "LeaveGame")]
    public async Task<ActionResult> LeaveGame(string gameCode, string playerId)
    {
        await _gameService.LeaveGame(gameCode, playerId);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/{votantId}/vote/{voteId}", Name = "Vote")]
    public async Task<ActionResult> Vote(string gameCode, string votantId, string voteId)
    {
        await _gameService.Vote(gameCode, votantId, voteId);
        
        return Ok();
    }

    [HttpPost("{gameCode}/end", Name = "EndGame")]
    public async Task<ActionResult> EndGame(string gameCode)
    {
        await _gameService.EndGame(gameCode);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/next", Name = "NextManche")]
    public async Task<ActionResult> NextManche(string gameCode)
    {
        await _gameService.NextRound(gameCode);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/results", Name = "EndRound")]
    public async Task<ActionResult> EndRound(string gameCode)
    {
        await _gameService.EndRound(gameCode);
        
        return Ok();
    }
}