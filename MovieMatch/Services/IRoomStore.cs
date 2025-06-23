using MovieMatch.Models;

namespace MovieMatch.Services
{
    public interface IRoomStore
    {
        Room? GetRoom(string roomId);
        void SaveRoom(Room room);
        void RemoveRoom(string roomId);
        IEnumerable<Room> GetAllRooms();
    }
}