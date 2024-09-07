using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;
public class InMemGamesRepository
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

    public IEnumerable<Game> GetAll() => games;

    public Game? Get(int id) => games.Find(g => g.Id == id);

    public void CreateGame(Game game)
    {
        game.Id = games.Max(g => g.Id) + 1;
        games.Add(game);
    }

    public void UpdateGame(Game updatedGame)
    {
        var index = games.FindIndex(g => g.Id == updatedGame.Id);
        games[index] = updatedGame;
    }

    public void DeleteGame(int id)
    {
        var index = games.FindIndex(g => g.Id == id);
        games.RemoveAt(index);
    }
}
