using MovieMatch.Enums;
using MovieMatch.Models;

namespace MovieMatch.Services
{
    public interface IMovieApiService
    {
        Task<List<Movie>> GetMoviesAsync(string[] services, ShowType showType, string? cursor = null);
    }
}