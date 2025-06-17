using MovieMatch.Models;

namespace MovieMatch.Services.Game
{
    public interface IGameStartService
    {
        Task<List<Movie>> StartGameAsync(string roomId);
    }
}