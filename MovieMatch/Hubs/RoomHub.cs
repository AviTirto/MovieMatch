using Microsoft.AspNetCore.SignalR;
using MovieMatch.Services.Game;
using MovieMatch.Models;

namespace MovieMatch.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IGameStartService _gameStartService;
        private static readonly Dictionary<string, HashSet<string>> RoomConnections = new();
        private static readonly Dictionary<string, HashSet<string>> RoomReadyStatus = new();

        public RoomHub(IGameStartService gameStartService)
        {
            _gameStartService = gameStartService;
        }

        public async Task JoinRoomGroup(string roomCode)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            if (!RoomConnections.ContainsKey(roomCode))
                RoomConnections[roomCode] = new();

            RoomConnections[roomCode].Add(Context.ConnectionId);

            await Clients.Group(roomCode).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveRoomGroup(string roomCode)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

            if (RoomConnections.TryGetValue(roomCode, out var connections))
            {
                connections.Remove(Context.ConnectionId);
                if (connections.Count == 0)
                {
                    RoomConnections.Remove(roomCode);
                    RoomReadyStatus.Remove(roomCode);
                }
            }

            await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task StartGame(string roomCode)
        {
            // Notify all users to switch screens
            await Clients.Group(roomCode).SendAsync("GameStarted");

            // Clear any previous readiness state
            RoomReadyStatus[roomCode] = new();
        }

        public async Task ReadyUp(string roomCode)
        {
            if (!RoomReadyStatus.ContainsKey(roomCode))
                RoomReadyStatus[roomCode] = new();

            RoomReadyStatus[roomCode].Add(Context.ConnectionId);

            if (RoomConnections.TryGetValue(roomCode, out var allConnections) &&
                RoomReadyStatus[roomCode].SetEquals(allConnections))
            {
                var movies = await _gameStartService.StartGameAsync(roomCode);
                await Clients.Group(roomCode).SendAsync("ReceiveMovies", movies);
            }
        }

        public async Task Swipe(string roomCode, string movieId)
        {
            // Placeholder for swipe logic
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var room in RoomConnections.Keys.ToList())
            {
                RoomConnections[room].Remove(Context.ConnectionId);
                RoomReadyStatus[room]?.Remove(Context.ConnectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
