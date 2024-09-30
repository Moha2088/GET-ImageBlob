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
