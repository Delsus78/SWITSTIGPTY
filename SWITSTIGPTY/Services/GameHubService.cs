using Microsoft.AspNetCore.SignalR;
namespace SWITSTIGPTY.Services;

public class GameHubService
{
    private readonly IHubContext<GameHub> _hubContext;
    private static readonly Random Rand = new();

    public GameHubService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewPlayerNumber(string groupName, string message)
    {
        await _hubContext.Clients.Group(groupName).SendAsync("player-number-changed", message);
    }
    
    public async Task SendToGroupExceptRandomAsync(string groupName, string emitName ,string message, string messageToExcept)
    {
        if (GameHub.GroupMembers.TryGetValue(groupName, out var members))
        {
            // Sélectionner un membre aléatoire
            var randomMember = members.ElementAt(Rand.Next(members.Count));

            // Envoyer le message au groupe sauf au membre sélectionné
            await _hubContext.Clients.GroupExcept(groupName, new List<string> { randomMember }).SendAsync(emitName, message);
            
            // Envoyer le message au membre sélectionné
            await _hubContext.Clients.Client(randomMember).SendAsync(emitName, messageToExcept);
        }
    }

    public void NotifyGameEnded(string groupName)
    {
        _hubContext.Clients.Group(groupName).SendAsync("game-ended");
    }
}