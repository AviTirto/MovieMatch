using MovieMatch.Enums;
using System.ComponentModel.DataAnnotations;

namespace MovieMatch.Models.DTOs
{
    public class CreateRoomRequest
    {
        [Required]
        public string HostName { get; set; } = string.Empty;
        [Required]
        public string[] Services { get; set; } = Array.Empty<string>();
        [Required]
        public ShowType ShowType { get; set; } = ShowType.Movie;
    }
}
