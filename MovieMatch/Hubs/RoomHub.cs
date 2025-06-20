using Microsoft.AspNetCore.SignalR;
using MovieMatch.Services.Game;
using MovieMatch.Services;
using MovieMatch.Models;

namespace MovieMatch.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IGameStartService _gameStartService;
        private readonly IRoomStore _roomStore;

        public RoomHub(IGameStartService gameStartService, IRoomStore roomStore)
        {
            _gameStartService = gameStartService;
            _roomStore = roomStore;
        }

        public async Task JoinRoomGroup(string roomCode, string userName)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            var room = _roomStore.GetRoom(roomCode);
            if (room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound", roomCode);
                return;
            }

            var person = room.People.FirstOrDefault(p => p.Name == userName);
            if (person == null)
            {
                await Clients.Caller.SendAsync("UserNotFound", userName);
                return;
            }

            person.ConnectionId = Context.ConnectionId;
            person.IsReady = false;

            await Clients.Group(roomCode).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task Ready(string roomCode)
        {
            var room = _roomStore.GetRoom(roomCode);
            if(room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound", roomCode);
                return;
            }

            var person = room.People.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (person == null)
            {
                await Clients.Caller.SendAsync("UserNotFound", Context.ConnectionId);
                return;
            }

            person.IsReady = true;
            await Clients.Group(roomCode).SendAsync("UserReady", Context.ConnectionId);
        }

        public async Task LeaveRoomGroup(string roomCode)
        {
            var room = _roomStore.GetRoom(roomCode);
            if (room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound", roomCode);
                return;
            }

            var person = room.People.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (person == null)
            {
                await Clients.Caller.SendAsync("UserNotFound", Context.ConnectionId);
                return;
            }
           
            room.People.Remove(person);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task StartGame(string roomCode)
        {
            var room = _roomStore.GetRoom(roomCode);
            if (room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound", roomCode);
                return;
            }

            if (!room.People.All(p => p.IsReady))
            {
                await Clients.Caller.SendAsync("NotAllReady", roomCode);
            }

            var movies = await _gameStartService.StartGameAsync(roomCode);
            await Clients.Group(roomCode).SendAsync("GameStarted");
            await Clients.Group(roomCode).SendAsync("RecieveMovies", movies);
        }

        public async Task Swipe(string roomCode, string movieId)
        {
            
        }
    }
}