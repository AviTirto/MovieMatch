namespace MovieMatch.Models
{
    public class MatchMetadata
    {
        public List<string> LikedBy { get; set; } = new();
        public Movie Movie { get; set; }
    }
}