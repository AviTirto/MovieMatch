using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MovieMatch.Enums;
using MovieMatch.Services;
using MovieMatch.Models;
using MovieMatch.Models.DTOs;
using Xunit;

public class MovieApiServiceTests
{
    private readonly MovieApiService _movieApiService;

    public MovieApiServiceTests()
    {
        var httpClient = new HttpClient();

        var configBuilder = new ConfigurationBuilder()
            .AddUserSecrets<MovieApiServiceTests>()
            .AddEnvironmentVariables()
            .Build();

        _movieApiService = new MovieApiService(httpClient, configBuilder);
    }

    [Fact]
    public async Task GetMoviesAsync_ReturnsNonEmptyList_AndLogsMovies()
    {
        var result = await _movieApiService.GetMoviesAsync(Array.Empty<string>(), ShowType.Movie);

        Assert.NotNull(result);
        Assert.NotEmpty(result.Movies);

        foreach (var movie in result.Movies)
        {
            Console.WriteLine(FormatMovieForLog(movie));
            Console.WriteLine(new string('-', 80));
        }

    }

    private string FormatMovieForLog(Movie movie)
    {
        return
            $"Id: {movie.Id}\n" +
            $"Title: {movie.Title}\n" +
            $"Overview: {movie.Overview}\n" +
            $"Directors: {string.Join(", ", movie.Director)}\n" +
            $"Genres: {string.Join(", ", movie.Genres)}\n" +
            $"Rating: {movie.Rating}\n" +
            $"Year: {movie.Year}\n" +
            $"PosterUrl: {movie.PosterUrl}\n" +
            $"Services: {string.Join(", ", movie.Services)}";
    }
}