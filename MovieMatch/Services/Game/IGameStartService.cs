namespace MovieMatch.Services.Game
{
    public interface IGameStartService
    {
        Task StartGameAsync(string roomId);
    }
}