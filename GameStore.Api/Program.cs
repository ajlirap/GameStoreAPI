using GameStore.Api.Authorization;
using GameStore.Api.Cors;
using GameStore.Api.Data;
using GameStore.Api.EndPoints;
using GameStore.Api.ErrorHandling;
using GameStore.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRepositories(builder.Configuration);

builder.Services.AddAuthentication()
                .AddJwtBearer()
                .AddJwtBearer("Auth0");
                
builder.Services.AddGameStoreAuthorization();
builder.Services.AddHttpLogging();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new (1.0);
    options.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddGameStoreCors(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.ConfigureExceptionHandler());
app.UseMiddleware<RequestTimingMiddleware>();


await app.Services.InitializeDbAsync();

app.UseHttpLogging();
app.UseCors();
app.MapGames();
app.Run();
