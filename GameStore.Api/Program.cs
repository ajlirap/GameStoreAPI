using Azure.Storage.Blobs;
using GameStore.Api.Authorization;
using GameStore.Api.Cors;
using GameStore.Api.Data;
using GameStore.Api.EndPoints;
using GameStore.Api.ErrorHandling;
using GameStore.Api.ImageUpload;
using GameStore.Api.Middleware;
using GameStore.Api.OpenAPI;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

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
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddGameStoreCors(builder.Configuration);

builder.Services.AddSwaggerGen()
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddEndpointsApiExplorer();

builder.Services.AddSingleton<IImageUploader>(
    new ImageUploader(
        new BlobContainerClient(
            builder.Configuration.GetConnectionString("AzureStorage"),
            builder.Configuration["AzureBlobStorage:ContainerName"]))
);

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.ConfigureExceptionHandler());
app.UseMiddleware<RequestTimingMiddleware>();


await app.Services.InitializeDbAsync();

app.UseHttpLogging();
app.UseCors();
app.MapGames();
app.MapImagesEndpoints();

app.UseGameStoreSwagger();

app.Run();
