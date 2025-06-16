using MovieMatch.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MovieMatch.Services.Game
{
    public class GameStartService : IGameStartService
    {
        private readonly IRoomStore _roomStore;
        private readonly IMovieApiService _apiService;
        private readonly IHubContext<RoomHub> _hubContext;

        public GameStartService(IRoomStore roomStore, IMovieApiService apiService, IHubContext<RoomHub> hubContext)
        {
            _roomStore = roomStore;
            _apiService = apiService;
            _hubContext = hubContext;
        }

        public async Task StartGameAsync(string roomId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null)
            {
                throw new Exception("Room not found");
            }

            // Fetch movies from the API
            var response = await _apiService.GetMoviesAsync([], room.ShowType);
        }
    }
}