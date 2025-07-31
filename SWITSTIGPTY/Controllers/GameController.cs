using Microsoft.AspNetCore.Mvc;
using SWITSTIGPTY.Models;
using SWITSTIGPTY.Services;

namespace SWITSTIGPTY.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController(GameService gameService) : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="numberOfManches"></param>
    /// <param name="pointsPerRightVote"></param>
    /// <param name="pointsPerVoteFooled"></param>
    /// <param name="pointsForImpostorFoundHimself"></param>
    /// <param name="isImpostorRevealedToHimself"></param>
    /// <returns></returns>
    [HttpGet(Name = "CreateGame")]
    public Game CreateGame(int numberOfManches, int pointsPerRightVote, int pointsPerVoteFooled, 
        int pointsForImpostorFoundHimself = 0, bool isImpostorRevealedToHimself = false)
    {
        return gameService.CreateGame(numberOfManches, pointsPerRightVote, pointsPerVoteFooled, 
            pointsForImpostorFoundHimself, isImpostorRevealedToHimself);
    }
    
    [HttpGet("{gameCode}", Name = "GetGame")]
    public Game GetGame(string gameCode)
    {
        return gameService.GetGame(gameCode);
    }

    [HttpGet("all", Name = "GetGames")]
    public IEnumerable<Game> GetGames()
    {
        return gameService.GetGames();
    }
    
    [HttpPost("{gameCode}/join", Name = "JoinGame")]
    public async Task<JoinGameDTO> JoinGame(string gameCode, string playerName)
    {
        return await gameService.JoinGame(gameCode, playerName);
    }
    
    [HttpPost("{gameCode}/reconnect", Name = "Reconnect")]
    public JoinGameDTO Reconnect(string gameCode, string playerId)
    {
        return gameService.ReconnectGame(gameCode, playerId);
    }
    
    [HttpPost("{gameCode}/leave", Name = "LeaveGame")]
    public async Task<ActionResult> LeaveGame(string gameCode, string playerId)
    {
        await gameService.LeaveGame(gameCode, playerId);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/{votantId}/vote/{voteId}", Name = "Vote")]
    public async Task<ActionResult> Vote(string gameCode, string votantId, string voteId)
    {
        await gameService.Vote(gameCode, votantId, voteId);
        
        return Ok();
    }

    [HttpPost("{gameCode}/end", Name = "EndGame")]
    public async Task<ActionResult> EndGame(string gameCode)
    {
        await gameService.EndGame(gameCode);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/next", Name = "NextManche")]
    public async Task<ActionResult> NextManche(string gameCode)
    {
        await gameService.NextRound(gameCode);
        
        return Ok();
    }
    
    [HttpPost("{gameCode}/results", Name = "EndRound")]
    public async Task<ActionResult> EndRound(string gameCode)
    {
        await gameService.EndRound(gameCode);
        
        return Ok();
    }
}