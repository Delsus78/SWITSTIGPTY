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

    [HttpGet(Name = "CreateGame")]
    public async Task<Game> CreateGame()
    {
        return await _gameService.CreateGame();
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
}