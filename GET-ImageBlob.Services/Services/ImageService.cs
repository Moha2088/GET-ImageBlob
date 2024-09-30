using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GET_ImageBlob.Services.Services.Interfaces;

namespace GET_ImageBlob.Services.Services;

public class ImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=cloudstoragemo;AccountKey=opISwGATdy6zr8sCExJf2qKT84R/gcQHdFxu7yxSlXMO/GOixmvizSnzg/6RYmMcXZZDIOwA4aw8+AStIu9NBQ==;EndpointSuffix=core.windows.net";
    private readonly string _blobContainer = "cloudstoragemocontainer";
    private readonly string _blobClient = "mo.jpg";
    
    public ImageService()
    {
        _blobServiceClient = new BlobServiceClient(_storageConnectionString);
    }

    public async Task<string> GetImageBlob()
    {
        string imageURL = $"https://{_blobServiceClient.AccountName}.blob.core.windows.net/{_blobContainer}/{_blobClient}";
        return imageURL;
    }
}
