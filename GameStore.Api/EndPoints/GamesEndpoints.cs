using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.EndPoints;
public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";
    public static RouteGroupBuilder MapGames(this IEndpointRouteBuilder endpoints)
    {

        var group = endpoints
        .MapGroup("/games")
        .WithParameterValidation();

        group.MapGet("/", (IGamesRepository repository) => repository.GetAll());

        group.MapGet("/{id}",
        (IGamesRepository repository, int id) =>
        {
            Game? game = repository.Get(id);
            return game is not null ? Results.Ok(game) : Results.NotFound();
        }
        )
        .WithName(GetGameEndpointName);

        group.MapPost("/", (IGamesRepository repository, Game game) =>
        {
            repository.CreateGame(game);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });

        group.MapPut("/{id}", (IGamesRepository repository, int id, Game game) =>
        {
            Game? existingGame = repository.Get(id);

            if (existingGame == null)
            {
                return Results.NotFound();
            }

            existingGame.Name = game.Name;
            existingGame.Genre = game.Genre;
            existingGame.Price = game.Price;
            existingGame.ReleaseDate = game.ReleaseDate;
            existingGame.ImageUri = game.ImageUri;

            repository.UpdateGame(existingGame);

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (IGamesRepository repository, int id) =>
        {
            Game? game = repository.Get(id);

            if (game is not null)
            {
                repository.DeleteGame(id);
            }
            return Results.NoContent();
        });
        return group;
    }
}
