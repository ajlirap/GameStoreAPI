using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace GameStore.Api.ImageUpload;

public class ImageUploader: IImageUploader
{
    private readonly BlobContainerClient containerClient;
    
    public ImageUploader(BlobContainerClient containerClient)
    {
        this.containerClient = containerClient;
    }

    public async Task<string> UploadImageAsync(IFormFile file)
    {
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
        
        var blobClient = containerClient.GetBlobClient(file.FileName);
        await blobClient.DeleteIfExistsAsync();

        using var filestream = file.OpenReadStream();

        await blobClient.UploadAsync(
            filestream, 
            new BlobHttpHeaders { ContentType = file.ContentType });

        return blobClient.Uri.ToString();
    }
}