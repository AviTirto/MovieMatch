using MovieMatch.Models;

namespace MovieMatch.Services
{
    public class InMemoryRoomStore : IRoomStore
    {
        private static readonly Dictionary<string, Room> _rooms = new();

        public Room? GetRoom(string roomId)
        {
            _rooms.TryGetValue(roomId, out var room);
            return room;
        }

        public void SaveRoom(Room room)
        {
            _rooms[room.Code] = room;
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return _rooms.Values;
        }
    }
}