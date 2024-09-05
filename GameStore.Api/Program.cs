using GameStore.Api.Entities;

List<Game> games = new()
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


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/games", () => games);

app.MapGet("/games/{id}",
(int id) =>
{
    Game? game = games.Find(g => g.Id == id);

    if (game == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(game);
}
);

app.Run();
