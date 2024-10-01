
using Azure.Storage.Blobs.Models;

namespace GET_ImageBlob.Services.Services.Interfaces;
public interface IImageService
{
    public Task<string> GetImageBlob();
    public Task UploadImageBlob(string localFilePath, CancellationToken cancellationToken);
}
