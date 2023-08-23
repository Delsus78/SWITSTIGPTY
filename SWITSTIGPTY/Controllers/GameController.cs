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
    public async Task<Game> CreateGame(string type, string? genre)
    {
        return await _gameService.CreateGame(type, genre);
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
    public async Task<Game> JoinGame(string gameCode, string playerName)
    {
        return await _gameService.JoinGame(gameCode, playerName);
    }
    
    [HttpPost("{gameCode}/leave", Name = "LeaveGame")]
    public async Task<ActionResult> LeaveGame(string gameCode)
    {
        await _gameService.LeaveGame(gameCode);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/start", Name = "StartGame")]
    public async Task<ActionResult> StartGame(string gameCode)
    {
        await _gameService.StartGame(gameCode);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/end", Name = "EndGame")]
    public ActionResult EndGame(string gameCode)
    {
        _gameService.EndGame(gameCode);
        
        return Ok();
    }
}