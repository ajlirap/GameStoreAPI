using GameStore.Api.EndPoints;
using GameStore.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IGamesRepository, InMemGamesRepository>();

var app = builder.Build();

app.MapGames();
app.Run();
