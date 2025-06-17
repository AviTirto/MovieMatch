namespace MovieMatch.Models
{
    public class Movie
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Overview { get; set; }
        public List<string> Director { get; set; } = new();
        public List<string> Genres { get; set; } = new();
        public int Rating { get; set; } 
        public int Year { get; set; }
        public string? PosterUrl { get; set; }
        public List<string> Services { get; set; } = new();

    }
}