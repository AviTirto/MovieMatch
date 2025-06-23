using Microsoft.AspNetCore.SignalR;
using MovieMatch.Services.Game;
using MovieMatch.Services;
using MovieMatch.Enums;
using MovieMatch.Models;

namespace MovieMatch.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IRoomStore _roomStore;
        private readonly IMovieFetchService _movieFetchService;

        public RoomHub(IRoomStore roomStore, IMovieFetchService movieFetchService)
        {
            _roomStore = roomStore;
            _movieFetchService = movieFetchService;
        }

        public async Task CreateRoom(string username, ShowType showType, string[] services)
        {
            var room = new Room
            {
                Services = services,
                ShowType = showType,
            };

            room.People.Add(new Person { Name = username, ConnectionId = Context.ConnectionId, IsReady = false });

            _roomStore.SaveRoom(room);

            await Groups.AddToGroupAsync(Context.ConnectionId, room.Code);
            await Clients.Caller.SendAsync("RoomMemberUpdate", room.People);
        }

        public async Task JoinRoom(string roomCode, string username)
        {
            var room = _roomStore.GetRoom(roomCode);
            if (room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound");
                return;
            }

            var person = new Person
            {
                Name = username,
                ConnectionId = Context.ConnectionId,
                IsReady = false
            };

            room.People.Add(person);

            await Groups.AddToGroupAsync(Context.ConnectionId, room.Code);
            await Clients.Group(room.Code).SendAsync("RoomMemberUpdate", room.People);

        }

        public async Task LeaveRoom(string roomCode)
        {
            var room = _roomStore.GetRoom(roomCode);
            if (room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound");
                return;
            }

            var person = room.People.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (person != null)
            {
                room.People.Remove(person);
                if (room.People.Count == 0)
                {
                    _roomStore.RemoveRoom(roomCode);
                }
                else
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
                    await Clients.Group(roomCode).SendAsync("RoomMemberUpdate", room.People);
                }
            }
        }

        public async Task Ready(string roomCode)
        {
            var room = _roomStore.GetRoom(roomCode);
            if (room == null)
            {
                await Clients.Caller.SendAsync("RoomNotFound");
                return;
            }

            var person = room.People.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (person != null)
            {
                person.IsReady = true;

                await Clients.Group(roomCode).SendAsync("RoomMemberUpdate", room.People);
            }
        }
    }
}
