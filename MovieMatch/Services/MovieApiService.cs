using System.Web;
using MovieMatch.Enums;
using System.Text.Json;
using MovieMatch.Models;
using MovieMatch.Models.DTOs;

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

        public async Task<MovieApiResult> GetMoviesAsync(string[] services, ShowType showType, string? cursor = null)
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

            if (cursor != null)
            {
                query["cursor"] = cursor;
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
                int rating = 0;
                if (show.TryGetProperty("rating", out var ratingProp))
                {
                    if (ratingProp.ValueKind == JsonValueKind.Number)
                        rating = ratingProp.GetInt32();
                    else if (ratingProp.ValueKind == JsonValueKind.String && int.TryParse(ratingProp.GetString(), out var r))
                        rating = r;
                }

                int year = 0;
                if (show.TryGetProperty("releaseYear", out var yearProp))
                {
                    if (yearProp.ValueKind == JsonValueKind.Number)
                        year = yearProp.GetInt32();
                    else if (yearProp.ValueKind == JsonValueKind.String && int.TryParse(yearProp.GetString(), out var y))
                        year = y;
                }

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
                    Rating = rating,
                    Year = year,
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

            var nextCursor = root.GetProperty("cursor").GetString();

            return new MovieApiResult
            {
                Movies = movies,
                Cursor = nextCursor
            };
        }
    }
}