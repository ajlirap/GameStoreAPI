using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public class InMemGamesRepository : IGamesRepository
{
    private readonly List<Game> games = new()
    {
    new Game
    {
        Id = 1,
        Name = "The Last of Us Part II",
        Genre = "Action-adventure",
        Price = 59.99m,
        ReleaseDate = new DateTime(2020, 6, 19),
        ImageUri = "https://placehold.co/100"
    },
    new Game
    {
        Id = 2,
        Name = "Cyberpunk 2077",
        Genre = "Action role-playing",
        Price = 49.94m,
        ReleaseDate = new DateTime(2020, 12, 10),
        ImageUri = "https://placehold.co/100"
    },
    new Game
    {
        Id = 3,
        Name = "Death Stranding",
        Genre = "Action",
        Price = 29.99m,
        ReleaseDate = new DateTime(2019, 11, 8),
        ImageUri = "https://placehold.co/100"
    }
    };

    public async Task<IEnumerable<Game>> GetAllAsync() => await Task.FromResult(games);

    public async Task<Game?> GetAsync(int id)
    {
        return await Task.FromResult(games.Find(g => g.Id == id));
    }

    public async Task CreateGameAsync(Game game)
    {
        game.Id = games.Max(g => g.Id) + 1;
        games.Add(game);
        await Task.CompletedTask;
    }

    public async Task UpdateGameAsync(Game updatedGame)
    {
        var index = games.FindIndex(g => g.Id == updatedGame.Id);
        games[index] = updatedGame;

        await Task.CompletedTask;
    }

    public async Task DeleteGameAsync(int id)
    {
        var index = games.FindIndex(g => g.Id == id);
        games.RemoveAt(index);

        await Task.CompletedTask;
    }
}
