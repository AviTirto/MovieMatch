using MovieMatch.Models;
using MovieMatch.Services;
using MovieMatch.Models.DTOs;

namespace MovieMatch.Services.Game
{
    public class MovieFetchService : IMovieFetchService
    {
        private readonly IMovieApiService _movieApiService;

    public MovieFetchService(IMovieApiService movieApiService)
    {
        _movieApiService = movieApiService;
    }

        public async Task<List<Movie>> FetchNextBatchAsync(Room room)
        {
            var ApiResult = await _movieApiService.GetMoviesAsync(room.Services, room.ShowType, room.Cursor);
            room.Cursor = ApiResult.Cursor;
            return ApiResult.Movies;
    }
}
}