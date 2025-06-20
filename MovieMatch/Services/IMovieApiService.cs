using MovieMatch.Enums;
using MovieMatch.Models;
using MovieMatch.DTOs;

namespace MovieMatch.Services
{
    public interface IMovieApiService
    {
        Task<MovieApiResult> GetMoviesAsync(string[] services, ShowType showType, string? cursor = null);
    }
}