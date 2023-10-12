using Microsoft.AspNetCore.SignalR;
namespace Qydha.Hubs;

public class ChatHub : Hub
{
    public void sendMessage(string name, string msg)
    {
        // save msg in DB

        // broadCast to all online clients;
        Clients.All.SendAsync("newMsg", name, msg);
    }
}
