using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.EndPoints;
public static class GamesEndpoints
{
    const string GetGameV1EndpointName = "GetGameV1";
    const string GetGameV2EndpointName = "GetGameV2";
    public static RouteGroupBuilder MapGames(this IEndpointRouteBuilder endpoints)
    {

        var group = endpoints.NewVersionedApi()
                             .MapGroup("/games")
                             .HasApiVersion(1.0)
                             .HasApiVersion(2.0)
                             .WithParameterValidation()
                             .WithOpenApi()
                             .WithTags("Games");

        group.MapGet("/", async (IGamesRepository repository,
        ILoggerFactory loggerFactory,
        [AsParameters] GetGamesDtoV1 request,
        HttpContext httpContext) 
         =>
        {
            var totalCount = await repository.CountAsync(request.Filter);
            httpContext.Response.AddPaginationHeaders(totalCount, request.PageSize);

            return Results.Ok((await repository.GetAllAsync(request.PageNumber, 
                                                            request.PageSize,
                                                            request.Filter))
                                               .Select(game => game.AsDtoV1()));
        })
        .MapToApiVersion(1.0)
        .WithSummary("Get all games")
        .WithDescription("Retrieves a list of games");


        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV1>, NotFound>> (IGamesRepository repository, int id) =>
        {

            Game? game = await repository.GetAsync(id);
            return game is not null ? TypedResults.Ok(game.AsDtoV1()) : TypedResults.NotFound();
        }
        )
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Get a game by id")
        .WithDescription("Retrieves a game by its id");


        group.MapGet("/", async (IGamesRepository repository, 
        ILoggerFactory loggerFactory,
        [AsParameters] GetGamesDtoV2 request,
        HttpContext httpContext
        ) 
        =>
        {
            var totalCount = await repository.CountAsync(request.Filter);
            httpContext.Response.AddPaginationHeaders(totalCount, request.PageSize);
            return Results.Ok(
                (await repository.GetAllAsync(request.PageSize, 
                                              request.PageNumber,
                                              request.Filter))
                .Select(game => game.AsDtoV2()));
        })
        .MapToApiVersion(2.0)
        .WithSummary("Get all games")
        .WithDescription("Retrieves a list of games");



        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV2>, NotFound>> (IGamesRepository repository, int id) =>
        {

            Game? game = await repository.GetAsync(id);
            return game is not null ? TypedResults.Ok(game.AsDtoV2()) : TypedResults.NotFound();
        }
        )
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(2.0)
        .WithSummary("Get a game by id")
        .WithDescription("Retrieves a game by its id");

        group.MapPost("/", async Task<CreatedAtRoute<GameDtoV1>> (IGamesRepository repository, CreateGameDto gameDto) =>
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
            return TypedResults.CreatedAtRoute(game.AsDtoV1() , GetGameV1EndpointName ,new { id = game.Id });
        })
       .RequireAuthorization(Policies.WriteAccess)
       .MapToApiVersion(1.0)
       .WithSummary("Create a new game")
       .WithDescription("Creates a new game");

        group.MapPut("/{id}", async Task<Results<NoContent,NotFound>> (IGamesRepository repository, int id, UpdateGameDto updatedGameDto) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame == null)
            {
                return TypedResults.NotFound();
            }

            existingGame.Name = updatedGameDto.Name;
            existingGame.Genre = updatedGameDto.Genre;
            existingGame.Price = updatedGameDto.Price;
            existingGame.ReleaseDate = updatedGameDto.ReleaseDate;
            existingGame.ImageUri = updatedGameDto.ImageUri;

            await repository.UpdateGameAsync(existingGame);

            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Update an existing game")
        .WithDescription("Updates an existing game");

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game is not null)
            {
                await repository.DeleteGameAsync(id);
            }
            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Delete a game")
        .WithDescription("Deletes a game");

        return group;
    }
}
