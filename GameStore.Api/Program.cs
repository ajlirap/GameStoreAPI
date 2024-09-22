using GameStore.Api.Data;
using GameStore.Api.EndPoints;
using GameStore.Api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories(builder.Configuration);
var app = builder.Build();

await app.Services.InitializeDbAsync();

app.MapGames();
app.Run();
