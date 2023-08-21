using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace SWITSTIGPTY.Services;


/// <summary>
/// rejoindre un groupe via le gameCode suivi du numero de joueur = 'gameCode-1'
/// </summary>
[SignalRHub]
public class GameHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has left the group {groupName}.");
    }

    public async Task SendMessageToGroup(string groupName, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", message);
    }
}