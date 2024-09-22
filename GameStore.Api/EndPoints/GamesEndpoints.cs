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

        group.MapGet("/", async (IGamesRepository repository) =>
            (await repository.GetAllAsync()).Select(game => game.AsDto()));

        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);
            return game is not null ? Results.Ok(game) : Results.NotFound();
        }
        )
        .WithName(GetGameEndpointName);

        group.MapPost("/", async (IGamesRepository repository, Game game) =>
        {
            await repository.CreateGameAsync(game);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });

        group.MapPut("/{id}", async (IGamesRepository repository, int id, Game game) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame == null)
            {
                return Results.NotFound();
            }

            existingGame.Name = game.Name;
            existingGame.Genre = game.Genre;
            existingGame.Price = game.Price;
            existingGame.ReleaseDate = game.ReleaseDate;
            existingGame.ImageUri = game.ImageUri;

            await repository.UpdateGameAsync(existingGame);

            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game is not null)
            {
                await repository.DeleteGameAsync(id);
            }
            return Results.NoContent();
        });
        return group;
    }
}
