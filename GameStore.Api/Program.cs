using System.Diagnostics;
using GameStore.Api.Authorization;
using GameStore.Api.Data;
using GameStore.Api.EndPoints;
using GameStore.Api.ErrorHandling;
using GameStore.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddGameStoreAuthorization();
builder.Services.AddHttpLogging();

builder.Services.AddApiVersioning();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.ConfigureExceptionHandler());
app.UseMiddleware<RequestTimingMiddleware>();


await app.Services.InitializeDbAsync();

app.UseHttpLogging();
app.MapGames();
app.Run();
