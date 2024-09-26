using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.ImageUpload;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.EndPoints;

public static class ImagesEndpoints
{
    public static RouteHandlerBuilder MapImagesEndpoints(this IEndpointRouteBuilder routes)
    {
        var api = routes.NewVersionedApi();

        return api.MapPost(
            "/images",
            async Task<Results<Ok<ImageUploadDto>, BadRequest>> (IFormFile file, IImageUploader imageUploader) =>
            {
                if (file.Length <= 0)
                {
                    return TypedResults.BadRequest();
                }

                var imageUri = await imageUploader.UploadImageAsync(file);

                return TypedResults.Ok(new ImageUploadDto(imageUri));
            })
            .DisableAntiforgery()
            .RequireAuthorization(Policies.WriteAccess)
            .HasApiVersion(1.0)
            .WithOpenApi()
            .WithSummary("Uploads an image")
            .WithDescription("Uploads an image to the storage container and return the URI of the uploaded image.")
            .WithTags("Images");
    }
}