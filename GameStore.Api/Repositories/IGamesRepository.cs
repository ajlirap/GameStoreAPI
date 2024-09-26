using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public interface IGamesRepository
{
    Task CreateGameAsync(Game game);
    Task DeleteGameAsync(int id);
    Task<Game?> GetAsync(int id);
    Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize);
    Task UpdateGameAsync(Game updatedGame);
    Task<int> CountAsync();
}
