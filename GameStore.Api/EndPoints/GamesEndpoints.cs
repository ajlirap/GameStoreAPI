using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;

namespace GameStore.Api.EndPoints;
public static class GamesEndpoints
{
    const string GetGameV1EndpointName = "GetGameV1";
    const string GetGameV2EndpointName = "GetGameV2";
    public static RouteGroupBuilder MapGames(this IEndpointRouteBuilder endpoints)
    {

        var v1Group = endpoints
        .MapGroup("/v1/games")
        .WithParameterValidation();

        var v2Group = endpoints
        .MapGroup("/v2/games")
        .WithParameterValidation();

        v1Group.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory) =>
        {
            return Results.Ok((await repository.GetAllAsync()).Select(game => game.AsDtoV1()));
        });


        v1Group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {

            Game? game = await repository.GetAsync(id);
            return game is not null ? Results.Ok(game.AsDtoV1()) : Results.NotFound();
        }
        )
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess);

        v2Group.MapGet("/", async (IGamesRepository repository, ILoggerFactory loggerFactory) =>
        {
            return Results.Ok((await repository.GetAllAsync()).Select(game => game.AsDtoV2()));
        });


        v2Group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {

            Game? game = await repository.GetAsync(id);
            return game is not null ? Results.Ok(game.AsDtoV2()) : Results.NotFound();
        }
        )
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess);

        v1Group.MapPost("/", async (IGamesRepository repository, CreateGameDto gameDto) =>
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
            return Results.CreatedAtRoute(GetGameV1EndpointName, new { id = game.Id }, game);
        })
       .RequireAuthorization(Policies.WriteAccess);

        v1Group.MapPut("/{id}", async (IGamesRepository repository, int id, UpdateGameDto updatedGameDto) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame == null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGameDto.Name;
            existingGame.Genre = updatedGameDto.Genre;
            existingGame.Price = updatedGameDto.Price;
            existingGame.ReleaseDate = updatedGameDto.ReleaseDate;
            existingGame.ImageUri = updatedGameDto.ImageUri;

            await repository.UpdateGameAsync(existingGame);

            return Results.NoContent();
        })
        .RequireAuthorization(Policies.ReadAccess);

        v1Group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game is not null)
            {
                await repository.DeleteGameAsync(id);
            }
            return Results.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess);

        return v1Group;
    }
}
