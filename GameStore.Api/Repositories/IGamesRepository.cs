using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public interface IGamesRepository
{
    IEnumerable<Game> GetAll();
    Game? Get(int id);
    void CreateGame(Game game);
    void UpdateGame(Game updatedGame);
    void DeleteGame(int id);
}
