using Azure;
using GET_ImageBlob.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GET_ImageBlob.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImagesController(IImageService imageService)
    {
        _imageService = imageService;
    }

    [HttpPost("single/{localFilePath}")]
    public async Task<IActionResult> UploadImageBlob([FromRoute] string localFilePath)
    {
        await _imageService.UploadImageBlob(localFilePath, HttpContext.RequestAborted);
        return Ok();
    }

    [HttpPost("bulk/{folderPath}")]
    public async Task<IActionResult> UploadImageBlobsBulk([FromRoute] string folderPath)
    {
        await _imageService.UploadImageBlobsBulk(folderPath, HttpContext.RequestAborted);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetImage()
    {
        try
        {
            var imageURL = await _imageService.GetImageBlob();
            return Content($@"
        <html>
            <body>
                <p>Here is your stored blob!</p>
                <img src='{imageURL}'/>   
            </body>
        </html>", "text/html");
            
        }
        
        catch (ArgumentOutOfRangeException)
        {
            return NotFound("No blobs found in this container");
        }
    }

    [HttpDelete("{blobName}")]
    public async Task<IActionResult> DeleteBlob([FromRoute] string blobName)
    {
        var response = await _imageService.DeleteBlob(blobName, HttpContext.RequestAborted);
        return Ok(response);
    }
}
