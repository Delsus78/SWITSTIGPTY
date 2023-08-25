using Microsoft.AspNetCore.SignalR;
using SWITSTIGPTY.Models;

namespace SWITSTIGPTY.Services;

public class GameHubService
{
    private readonly IHubContext<GameHub> _hubContext;

    public GameHubService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewPlayerNumber(string groupName, string message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("player-number-changed", message);
    }
    
    public async Task SendToGroupExceptListAsync(string groupName, string emitName ,object message, object messageToExcept, List<string> playersIdToExcept)
    {
        if (GameHub.GroupMembers.TryGetValue(groupName, out var members))
        {
            // Sélectionner un membre aléatoire
            var exceptedConnIds = GameHub.GroupMembers[groupName]
                .Where(x => playersIdToExcept.Contains(x.Item2))
                .Select(x => x.Item1)
                .ToList();

            // Envoyer le message au groupe sauf au membre sélectionné
            await _hubContext.Clients.GroupExcept(groupName, exceptedConnIds).SendAsync(emitName, message);
            
            // Envoyer le message aux membres sélectionnés
            await _hubContext.Clients.Clients(exceptedConnIds).SendAsync(emitName, messageToExcept);
        }
    }

    public void NotifyGameEnded(string groupName, List<Player> players)
    {
        _hubContext.Clients.Group(groupName).SendAsync("game-ended", players);
    }
    
    public async Task NotifyNewVote(string groupName, string playerId)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("new-vote", playerId);
    }
}