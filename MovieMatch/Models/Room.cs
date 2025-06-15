namespace MovieMatch.Models
{
    public class Room
    {
        public string Code { get; set; } = Guid.NewGuid().ToString("N")[..6].ToUpper();
        public List<Person> People { get; set; } = new();
        public List<Movie> MovieQueue { get; set; } = new();
        public Dictionary<string, List<string>> SwipeMap { get; set; } = new();
        public List<string> StreamingServices { get; set; } = new();
    }
}