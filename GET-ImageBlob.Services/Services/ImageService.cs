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
        localFilePath = localFilePath.Replace('"', ' ').Trim();
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

    private bool IsValid(string file)
    {
        return Path.GetFileName(file).EndsWith("jpg") || Path.GetFileName(file).EndsWith("png") || Path.GetFileName(file).EndsWith("gif");
    }
        
    public async Task UploadImageBlobsBulk(string folderPath, CancellationToken cancellationToken)
    {
        folderPath = folderPath.Replace('"', ' ').Trim();
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);

        foreach(var file in Directory.GetFiles(folderPath))
        {
            if (!IsValid(file)) continue;
            BlobClient blobClient = blobContainerClient.GetBlobClient(file);
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

    public async Task<Response<bool>> DeleteBlob(string blobName, CancellationToken cancellationToken)
    {
        BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(_blobContainer);
        var name = blobContainerClient.GetBlobs()
            .SingleOrDefault(blob => blob.Name.StartsWith(blobName))?.Name;
        
        BlobClient blobClient = blobContainerClient.GetBlobClient(name);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken:cancellationToken, snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots);
        return response;
    }
}
