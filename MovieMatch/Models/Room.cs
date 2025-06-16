using MovieMatch.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieMatch.Models
{
    public class Room
    {
        public string Code { get; set; } = Guid.NewGuid().ToString("N")[..6].ToUpper();
        public List<Person> People { get; set; } = new();
        public Dictionary<string, MatchMetadata> SwipeMap { get; set; } = new();
        [Required]
        public string[] Services { get; set; } = Array.Empty<string>();
        public string? Cursor { get; set; }
        public ShowType ShowType { get; set; } = ShowType.Movie;
    }
}