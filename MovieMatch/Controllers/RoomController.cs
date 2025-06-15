using Microsoft.AspNetCore.Mvc;
using MovieMatch.Models;
using MovieMatch.DTOs;

namespace MovieMatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private static readonly Dictionary<string, Room> Rooms = new();

        [HttpPost("create")]
        public IActionResult CreateRoom([FromBody] CreateRoomRequest request)
        {
            var room = new Room
            {
                StreamingServices = request.StreamingServices
            };

            Rooms[room.Code] = room;

            return Ok(new { room.Code, room.StreamingServices });
        }

        [HttpPost("{code}/join")]
        public IActionResult Joinroom(string code, [FromBody] JoinRoomRequest request)
        {
            if (!Rooms.TryGetValue(code, out var room))
            {
                return NotFound($"Room {code} not found.");
            }

            var person = new Person { Name = request.Name };

            if (!room.People.Any(p => p.Name == person.Name))
            {
                room.People.Add(person);
            }

            return Ok(new { message = $"{request.Name} joined room {code}.", people = room.People });
        }
    }
}