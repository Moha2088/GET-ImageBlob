using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GET_ImageBlob.Services.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Text;

namespace GET_ImageBlob.Services.Services;

public class ImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _storageConnectionString;
    private readonly string _blobContainer;
    
    public ImageService() 
    {
        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _storageConnectionString = config.GetSection("AzureCredentials:StorageConnectionString").Value!;
        _blobContainer = config.GetSection("AzureCredentials:BlobContainerName").Value!;
        _blobServiceClient = new BlobServiceClient(_storageConnectionString);
    }

    public async Task UploadImageBlob(string localFilePath, CancellationToken cancellationToken)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);
        string fileName = Path.GetFileName(localFilePath);
        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
        Response<BlobContentInfo>? uploadedBlobState = null;
        FileStream stream = File.OpenRead(localFilePath);

        try
        {
            uploadedBlobState = await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
        }

        catch (RequestFailedException e) 
        {
            Log.Information("Error uploading blob: {@Error}", e.ToString());
        }

        finally
        {
            Log.Information("Upload finished: {@State}", uploadedBlobState);
        }
    }

    public async Task UploadImageBlobsBulk(string localFilePath, CancellationToken cancellationToken)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);
        string fileName = Path.GetFileName(localFilePath);
        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
                
        foreach(var file in Directory.GetFiles(localFilePath))
        {
            //if (!file.EndsWith(".jpg") || !file.EndsWith(".png"))
            //{
            //    continue;
            //}

            FileStream stream = File.OpenRead(file);
            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken);
        }
    }

    public Task<string> GetImageBlob()
    {
        Random random = new Random();
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);
        var randomBlobIdx = random.Next(0, blobContainerClient.GetBlobs().Count());
        var randomBlob = blobContainerClient.GetBlobs().ToList()[randomBlobIdx].Name;
        string imageURL = $"https://{_blobServiceClient.AccountName}.blob.core.windows.net/{_blobContainer}/{randomBlob}";
        Log.Information("Retrieved URL: {@URL}", imageURL);
        return Task.FromResult(imageURL);
    }
}
