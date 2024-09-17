
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

    public IEnumerable<Game> GetAll()
    {
        return dbContext.Games.AsNoTracking().ToList();
    }
    
    public Game? Get(int id)
    {
        return dbContext.Games.Find(id);
    }
    public void CreateGame(Game game)
    {
        dbContext.Games.Add(game);
        dbContext.SaveChanges();
    }
    
    public void UpdateGame(Game updatedGame)
    {
        dbContext.Games.Update(updatedGame);
        dbContext.SaveChanges();
    }

    public void DeleteGame(int id)
    {
        dbContext.Games.Where(game => game.Id == id)
                        .ExecuteDelete();
    }
}