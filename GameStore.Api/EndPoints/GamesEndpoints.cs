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

        var group = endpoints.NewVersionedApi()
                             .MapGroup("/games")
                             .HasApiVersion(1.0)
                             .HasApiVersion(2.0)
                             .WithParameterValidation();

        group.MapGet("/", async (IGamesRepository repository,
        ILoggerFactory loggerFactory,
        [AsParameters] GetGamesDtoV1 request,
        HttpContext httpContext) 
         =>
        {
            var totalCount = await repository.CountAsync();
            httpContext.Response.AddPaginationHeaders(totalCount, request.PageSize);

            return Results.Ok((await repository.GetAllAsync(request.PageNumber, request.PageSize))
                                               .Select(game => game.AsDtoV1()));
        })
        .MapToApiVersion(1.0);


        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {

            Game? game = await repository.GetAsync(id);
            return game is not null ? Results.Ok(game.AsDtoV1()) : Results.NotFound();
        }
        )
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0);


        group.MapGet("/", async (IGamesRepository repository, 
        ILoggerFactory loggerFactory,
        [AsParameters] GetGamesDtoV2 request,
        HttpContext httpContext
        ) 
        =>
        {
            var totalCount = await repository.CountAsync();
            httpContext.Response.AddPaginationHeaders(totalCount, request.PageSize);
            return Results.Ok(
                (await repository.GetAllAsync(request.PageSize, request.PageNumber))
                .Select(game => game.AsDtoV2()));
        })
        .MapToApiVersion(2.0);



        group.MapGet("/{id}", async (IGamesRepository repository, int id) =>
        {

            Game? game = await repository.GetAsync(id);
            return game is not null ? Results.Ok(game.AsDtoV2()) : Results.NotFound();
        }
        )
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(2.0);

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
            return Results.CreatedAtRoute(GetGameV1EndpointName, new { id = game.Id }, game);
        })
       .RequireAuthorization(Policies.WriteAccess)
       .MapToApiVersion(1.0);

        group.MapPut("/{id}", async (IGamesRepository repository, int id, UpdateGameDto updatedGameDto) =>
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
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0);

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);

            if (game is not null)
            {
                await repository.DeleteGameAsync(id);
            }
            return Results.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0);

        return group;
    }
}
