using System.Web;
using MovieMatch.Enums;
using System.Text.Json;
using MovieMatch.Models;

namespace MovieMatch.Services
{
    public class MovieApiService : IMovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private const string BaseUrl = "https://streaming-availability.p.rapidapi.com/shows/search/filters";
        private const string ApiHost = "streaming-availability.p.rapidapi.com";

        public MovieApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["ApiKey"] ?? throw new ArgumentNullException("API Key", "API Key is not configured.");
        }

        public async Task<List<Movie>> GetMoviesAsync(string[] services, ShowType showType, string? cursor = null)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["country"] = "us";
            query["series_granularity"] = "show";
            query["order_by"] = "rating";
            query["output_language"] = "en";

            if (showType == ShowType.Movie)
            {
                query["show_type"] = "movie";
            }

            var url = $"{BaseUrl}?{query}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", _apiKey);
            request.Headers.Add("x-rapidapi-host", ApiHost);

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            List<Movie> movies = new();
            var shows = root.GetProperty("shows");

            foreach (var show in shows.EnumerateArray())
            {
                var movie = new Movie
                {
                    Id = show.GetProperty("id").GetString(),
                    Title = show.GetProperty("title").GetString(),
                    Overview = show.GetProperty("overview").GetString(),
                    Director = show.GetProperty("directors")
                        .EnumerateArray()
                        .Select(d => d.GetString())
                        .ToList(),
                    Genres = show.GetProperty("genres")
                        .EnumerateArray()
                        .Select(g => g.GetProperty("id").GetString())
                        .ToList(),
                    Rating = int.TryParse(show.GetProperty("rating").GetString(), out int rating) ? rating : 0,
                    Year = int.TryParse(show.GetProperty("releaseYear").GetString(), out int year) ? year : 0,
                    PosterUrl = show.GetProperty("imageSet")
                        .GetProperty("verticalPoster")
                        .GetProperty("w480")
                        .GetString(),
                    Services = show.GetProperty("streamingOptions")
                        .GetProperty("us")
                        .EnumerateArray()
                        .Select(s => s.GetProperty("service")
                                      .GetProperty("id")
                                      .GetString()
                        )
                        .ToList()
                };

                movies.Add(movie);
            }

            return movies;
        }
    }
}