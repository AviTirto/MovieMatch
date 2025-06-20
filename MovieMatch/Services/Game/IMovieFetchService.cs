using MovieMatch.Models;

namespace MovieMatch.Services.Game
{
    public interface IMovieFetchService
    {
        Task<List<Movie>> FetchNextBatchAsync(Room room);
    }
}