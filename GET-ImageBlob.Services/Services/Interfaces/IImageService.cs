
using Azure.Storage.Blobs.Models;

namespace GET_ImageBlob.Services.Services.Interfaces;
public interface IImageService
{
    public Task<string> GetImageBlob();
    public Task UploadImageBlob(string localFilePath, CancellationToken cancellationToken);

    /// <summary>
    /// Upload multiple images from a folder
    /// </summary>
    /// <param name="localFilePath">The path to the folder with the images</param>
    /// <param name="cancellationToken"></param>
    public Task UploadImageBlobsBulk(string localFilePath, CancellationToken cancellationToken);
}
