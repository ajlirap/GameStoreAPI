using System.Diagnostics;
using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
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

        group.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory) =>
        {
            try
            {
                return Results.Ok((await repository.GetAllAsync()).Select(game => game.AsDto()));
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger("Games Endpoints");
                logger.LogError(ex, "Could not process a request on machine {MachineName}. TraceId: {TraceId}", Environment.MachineName,
                Activity.Current?.TraceId);

                return Results.Problem(
                    title: "We make a mistake but we are working on it!",
                    statusCode: StatusCodes.Status500InternalServerError,
                    extensions: new Dictionary<string, object?>
                    {
                        {"traceId", Activity.Current?.TraceId.ToString()}
                    }
                );
            }
        });


        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);
            return game is not null ? Results.Ok(game) : Results.NotFound();
        }
        )
        .WithName(GetGameEndpointName)
        .RequireAuthorization(Policies.ReadAccess);

        group.MapPost("/", async (IGamesRepository repository, CreateGameDto gameDto) =>
        {
            Game game = new()
            {
                Name = gameDto.Name,
                Genre = gameDto.Genre,
                Price = gameDto.Price,
                ReleaseDate = gameDto.ReleaseDate,
                ImageUri = gameDto.ImageUri
            };
            await repository.CreateGameAsync(game);
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        })
       .RequireAuthorization(Policies.WriteAccess);

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
        })
        .RequireAuthorization(Policies.ReadAccess);

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game is not null)
            {
                await repository.DeleteGameAsync(id);
            }
            return Results.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess);

        return group;
    }
}
