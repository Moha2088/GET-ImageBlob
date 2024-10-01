using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GET_ImageBlob.Services.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GET_ImageBlob.Services.Services;

public class ImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=cloudstoragemo;AccountKey=opISwGATdy6zr8sCExJf2qKT84R/gcQHdFxu7yxSlXMO/GOixmvizSnzg/6RYmMcXZZDIOwA4aw8+AStIu9NBQ==;EndpointSuffix=core.windows.net";
    private readonly string _blobContainer = "cloudstoragemocontainer";
    private readonly ILogger<ImageService> _logger;
    
    public ImageService(ILogger<ImageService> logger) 
    {
        _blobServiceClient = new BlobServiceClient(_storageConnectionString);
        _logger = logger;
    }

    public async Task UploadImageBlob(string localFilePath, CancellationToken cancellationToken)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);
        BlobClient blobClient = blobContainerClient.GetBlobClient(localFilePath);
        

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(localFilePath));

        try
        {
            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
        }

        catch (RequestFailedException e) 
        {   
            _logger.LogWarning($"Error uploading blob to storage:\nError Message: {e.ToString()}");
        }

        finally
        {
            _logger.LogInformation("Upload finished");
        }
    }

    public Task<string> GetImageBlob()
    {
        Random random = new Random();
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);
        var randomBlobIdx = random.Next(0, blobContainerClient.GetBlobs().Count() - 1);
        var randomBlob = blobContainerClient.GetBlobs().ToList()[randomBlobIdx].Name;
        string imageURL = $"https://{_blobServiceClient.AccountName}.blob.core.windows.net/{_blobContainer}/{randomBlob}";
        return Task.FromResult(imageURL);
    }
}
