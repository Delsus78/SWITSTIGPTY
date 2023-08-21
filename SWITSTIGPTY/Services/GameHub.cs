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
    public static readonly ConcurrentDictionary<string, HashSet<string>> GroupMembers = new();

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Ajouter le membre au groupe dans la collection
        GroupMembers.AddOrUpdate(groupName, new HashSet<string> { Context.ConnectionId }, (_, existingValue) =>
        {
            existingValue.Add(Context.ConnectionId);
            return existingValue;
        });
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

        // Supprimer le membre du groupe dans la collection
        if (GroupMembers.TryGetValue(groupName, out var members))
        {
            members.Remove(Context.ConnectionId);
        }
    }
}