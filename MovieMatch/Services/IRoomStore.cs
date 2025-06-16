using MovieMatch.Models;

namespace MovieMatch.Services
{
    public interface IRoomStore
    {
        Room? GetRoom(string roomId);
        void SaveRoom(Room room);
        IEnumerable<Room> GetAllRooms();
    }
}