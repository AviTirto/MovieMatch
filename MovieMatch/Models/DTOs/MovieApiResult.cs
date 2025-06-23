namespace MovieMatch.Models.DTOs
{
    public class MovieApiResult
    {
        public List<Movie> Movies { get; set; } = new();
        public string? Cursor { get; set; }
    }
}