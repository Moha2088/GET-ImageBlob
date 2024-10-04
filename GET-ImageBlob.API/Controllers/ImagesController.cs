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

    [HttpPost("bulk")]
    public async Task<IActionResult> UploadImageBlobsBulk(string localFilePath)
    {
        await _imageService.UploadImageBlobsBulk(localFilePath, HttpContext.RequestAborted);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetImage()
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
}
