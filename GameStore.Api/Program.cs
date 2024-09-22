using GameStore.Api.Authorization;
using GameStore.Api.Data;
using GameStore.Api.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddAuthentication().AddJwtBearer();

builder.Services.AddGameStoreAuthorization();

builder.Logging.AddJsonConsole(options => 
{
    options.JsonWriterOptions = new() { Indented = true };
});

var app = builder.Build();

await app.Services.InitializeDbAsync();

app.MapGames();
app.Run();
