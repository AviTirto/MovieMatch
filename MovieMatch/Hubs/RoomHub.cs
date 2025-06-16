using Microsoft.AspNetCore.SignalR;
using MovieMatch.Services;

namespace MovieMatch.Hubs
{
    public class RoomHub : Hub
    {
        public async Task JoinRoomGroup(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveRoomGroup(string roomCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task StartGame(string roomCode)
        {
            await Clients.Group(roomCode).SendAsync("GameStarted");
        }

        public async Task Swipe(string roomCode, string movieId)
        {
            
        }
    }
}