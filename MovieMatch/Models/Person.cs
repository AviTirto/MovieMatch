namespace MovieMatch.Models
{
    public class Person
    {
        public string Name { get; set; }
        public bool IsReady { get; set; } = false;
        public string? ConnectionId { get; set; }
    }
}