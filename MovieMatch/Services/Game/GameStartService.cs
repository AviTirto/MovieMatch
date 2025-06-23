using MovieMatch.Models;

namespace MovieMatch.Services.Game
{
    public class GameStartService : IGameStartService
    {
        private readonly IRoomStore _roomStore;
        private readonly IMovieApiService _apiService;


        public GameStartService(IRoomStore roomStore, IMovieApiService apiService)
        {
            _roomStore = roomStore;
            _apiService = apiService;
        }

        public async Task<List<Movie>> StartGameAsync(string roomId)
        {
            var room = _roomStore.GetRoom(roomId);
            if (room == null)
            {
                throw new Exception("Room not found");
            }

            // Fetch movies from the API
            var ApiResult = await _apiService.GetMoviesAsync([], room.ShowType);

            return ApiResult.Movies;
        }
    }
}