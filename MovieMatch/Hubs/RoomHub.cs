using Microsoft.AspNetCore.SignalR;

namespace MovieMatch.Hubs
{
    public class RoomHub : Hub
    {
        public async Task JoinRoomGroup(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("UserJoined", Context.ConnectionId);
        }
    }
}