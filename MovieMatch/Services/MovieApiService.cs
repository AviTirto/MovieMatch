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

            if (!string.IsNullOrEmpty(cursor))
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

            var movies = new List<Movie>();

            if (root.TryGetProperty("shows", out var shows) && shows.ValueKind == JsonValueKind.Array)
            {
                foreach (var show in shows.EnumerateArray())
                {
                    // Safe parsing helpers
                    static string? GetStringSafe(JsonElement elem, string propertyName)
                    {
                        return elem.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String ? prop.GetString() : null;
                    }

                    static int GetIntSafe(JsonElement elem, string propertyName)
                    {
                        if (elem.TryGetProperty(propertyName, out var prop))
                        {
                            if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out int val))
                                return val;
                            if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out val))
                                return val;
                        }
                        return 0;
                    }

                    static List<string> GetStringListSafe(JsonElement elem, string propertyName)
                    {
                        var list = new List<string>();
                        if (elem.TryGetProperty(propertyName, out var arrayProp) && arrayProp.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in arrayProp.EnumerateArray())
                            {
                                if (item.ValueKind == JsonValueKind.String)
                                    list.Add(item.GetString()!);
                            }
                        }
                        return list;
                    }

                    static List<string> GetNestedStringListSafe(JsonElement elem, string arrayPropertyName, string nestedPropertyName)
                    {
                        var list = new List<string>();
                        if (elem.TryGetProperty(arrayPropertyName, out var arrayProp) && arrayProp.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in arrayProp.EnumerateArray())
                            {
                                if (item.TryGetProperty(nestedPropertyName, out var nestedProp) && nestedProp.ValueKind == JsonValueKind.String)
                                {
                                    list.Add(nestedProp.GetString()!);
                                }
                            }
                        }
                        return list;
                    }

                    static string? GetNestedPropertyStringSafe(JsonElement elem, params string[] propertyPath)
                    {
                        JsonElement current = elem;
                        foreach (var prop in propertyPath)
                        {
                            if (!current.TryGetProperty(prop, out current))
                                return null;
                        }
                        return current.ValueKind == JsonValueKind.String ? current.GetString() : null;
                    }

                    var movie = new Movie
                    {
                        Id = GetStringSafe(show, "id"),
                        Title = GetStringSafe(show, "title"),
                        Overview = GetStringSafe(show, "overview"),
                        Director = GetStringListSafe(show, "directors"),
                        Genres = new List<string>(), 
                        Rating = GetIntSafe(show, "rating"),
                        Year = GetIntSafe(show, "releaseYear"),
                        PosterUrl = GetNestedPropertyStringSafe(show, "imageSet", "verticalPoster", "w480"),
                        Services = new List<string>() 
                    };

                    if (show.TryGetProperty("genres", out var genresProp) && genresProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var genre in genresProp.EnumerateArray())
                        {
                            var genreId = GetStringSafe(genre, "id");
                            if (genreId != null)
                                movie.Genres.Add(genreId);
                        }
                    }

                    if (show.TryGetProperty("streamingOptions", out var streamingOptions) &&
                        streamingOptions.TryGetProperty("us", out var usOptions) &&
                        usOptions.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var option in usOptions.EnumerateArray())
                        {
                            var serviceId = GetNestedPropertyStringSafe(option, "service", "id");
                            if (serviceId != null)
                                movie.Services.Add(serviceId);
                        }
                    }

                    if (services != null && services.Length > 0)
                    {
                        if (!movie.Services.Any(svc => services.Contains(svc, StringComparer.OrdinalIgnoreCase)))
                            continue;
                    }

                    movies.Add(movie);
                }
            }

            var nextCursor = root.TryGetProperty("nextCursor", out var cursorProp) && cursorProp.ValueKind == JsonValueKind.String
                ? cursorProp.GetString()
                : null;

            return new MovieApiResult
            {
                Movies = movies,
                Cursor = nextCursor
            };
        }
    }
}
