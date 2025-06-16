using MovieMatch.Enums;

namespace MovieMatch.Services
{
    public interface IMovieApiService
    {
        Task<string> GetMoviesAsync(string[] services, ShowType showType, string? cursor = null);
    }
}