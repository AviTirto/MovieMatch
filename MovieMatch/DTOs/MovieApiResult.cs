using MovieMatch.Models;

namespace MovieMatch.DTOs
{
    public class MovieApiResult
    {
        public List<Movie> Movies { get; set; } = new();
        public string? Cursor { get; set; }
    }
}