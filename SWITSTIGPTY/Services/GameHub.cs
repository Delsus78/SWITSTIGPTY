using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace SWITSTIGPTY.Services;


/// <summary>
/// Hub SignalR pour les parties de jeu
/// </summary>
[SignalRHub]
public class GameHub : Hub
{
    public static readonly ConcurrentDictionary<string, HashSet<Tuple<string, string>>> GroupMembers = new();

    public async Task JoinGroup(string groupName, string playerId)
    {
        // reconnecter le joueur au groupe s'il est déjà connecté
        if (GroupMembers.TryGetValue(groupName, out var members))
        {
            if (members.Any(x => x.Item2 == playerId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                
                // change the connection id of the player
                members.RemoveWhere(x => x.Item2 == playerId);
                members.Add(new Tuple<string, string>(Context.ConnectionId, playerId));

                return;
            }
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Ajouter le membre au groupe dans la collection
        GroupMembers.AddOrUpdate(
            groupName, 
            new HashSet<Tuple<string, string>> {new (Context.ConnectionId, playerId)}, 
            (_, existingValue) =>
        {
            existingValue.Add(new Tuple<string, string>(Context.ConnectionId, playerId));
            return existingValue;
        });
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        // Supprimer le membre du groupe dans la collection
        if (GroupMembers.TryGetValue(groupName, out var members))
        {
            members.RemoveWhere(x => x.Item1 == Context.ConnectionId);
        }
    }
}