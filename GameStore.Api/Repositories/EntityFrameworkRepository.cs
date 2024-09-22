
using GameStore.Api.Data;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class EntityFrameworkRepository : IGamesRepository
{
    private readonly GameStoreContext dbContext;

    public EntityFrameworkRepository(GameStoreContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        return await dbContext.Games.AsNoTracking().ToListAsync();
    }
    
    public async Task<Game?> GetAsync(int id)
    {
        return await dbContext.Games.FindAsync(id);
    }
    public async Task CreateGameAsync(Game game)
    {
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task UpdateGameAsync(Game updatedGame)
    {
        dbContext.Games.Update(updatedGame);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteGameAsync(int id)
    {
        await dbContext.Games.Where(game => game.Id == id)
                             .ExecuteDeleteAsync();
    }
}