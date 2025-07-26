using Microsoft.AspNetCore.SignalR;
using SWITSTIGPTY.Models;

namespace SWITSTIGPTY.Services;

public class GameHubService(IHubContext<GameHub> hubContext, ILogger<GameHubService> logger)
{

    public async Task NotifyNewPlayerNumber(string groupName, string message)
    {
        logger.LogInformation("Sending player number change notification to group {GroupName}: {Message}", groupName, message);
        await hubContext.Clients.Group(groupName).SendAsync("player-number-changed", message);
    }
    
    public async Task SendToGroupExceptListAsync(string groupName, string emitName ,object message, object messageToExcept, List<string> playersIdToExcept)
    {
        logger.LogInformation("Sending message to group {GroupName} except players: {PlayersIdToExcept}", groupName, string.Join(", ", playersIdToExcept));
        
        if (GameHub.GroupMembers.TryGetValue(groupName, out _))
        {
            // Sélectionner un membre aléatoire
            var exceptedConnIds = GameHub.GroupMembers[groupName]
                .Where(x => playersIdToExcept.Contains(x.Item2))
                .Select(x => x.Item1)
                .ToList();

            // Envoyer le message au groupe sauf au membre sélectionné
            await hubContext.Clients.GroupExcept(groupName, exceptedConnIds).SendAsync(emitName, message);
            
            // Envoyer le message aux membres sélectionnés
            await hubContext.Clients.Clients(exceptedConnIds).SendAsync(emitName, messageToExcept);
        }
    }

    public async Task NotifyGameEnded(string groupName, List<Player> players)
    {
        logger.LogInformation("Notifying game ended for group {GroupName} with players: {Players}", groupName, string.Join(", ", players.Select(p => p.Name)));
        
        await hubContext.Clients.Group(groupName).SendAsync("game-ended", players);
    }
    
    public async Task NotifyNewVote(string groupName, string playerId)
    {
        logger.LogInformation("Notifying new vote for player {PlayerId} in group {GroupName}", playerId, groupName);
        
        await hubContext.Clients.Group(groupName).SendAsync("new-vote", playerId);
    }
    
    public async Task NotifyEndRound(string groupeName, List<Player> players)
    {
        logger.LogInformation("Notifying end of round for group {GroupName} with players: {Players}", groupeName, string.Join(", ", players.Select(p => p.Name)));
        
        await hubContext.Clients.Group(groupeName).SendAsync("end-round", players);
    }
    
    public void LeaveGroup(string groupName, string playerId)
    {
        logger.LogInformation("Player {PlayerId} is leaving group {GroupName}", playerId, groupName);
        
        // Supprimer le membre du groupe dans la collection
        if (GameHub.GroupMembers.TryGetValue(groupName, out var members))
        {
            members.RemoveWhere(x => x.Item2 == playerId);
        }
    }
}