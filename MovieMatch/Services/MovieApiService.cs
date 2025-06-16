using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using MovieMatch.Enums;

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

        public async Task<string> GetMoviesAsync(string[] services, ShowType showType, string? cursor = null)
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
            return body;
        }
    }
}