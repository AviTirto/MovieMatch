using Microsoft.AspNetCore.Mvc;
using MovieMatch.Models;
using MovieMatch.DTOs;
using MovieMatch.Services;

namespace MovieMatch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomStore _roomStore;

        public RoomController(IRoomStore roomStore)
        {
            _roomStore = roomStore;
        }

        [HttpPost("create")]
        public IActionResult CreateRoom([FromBody] CreateRoomRequest request)
        {

            var room = new Room
            {
                Services = request.Services,
                ShowType = request.ShowType
            };

            room.People.Add(new Person { Name = request.HostName });

            _roomStore.SaveRoom(room);

            return Ok(new { room.Code, room.Services });

        }

        [HttpPost("{code}/join")]
        public IActionResult Joinroom(string code, [FromBody] JoinRoomRequest request)
        {
            var room = _roomStore.GetRoom(code);
            if (room == null)
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