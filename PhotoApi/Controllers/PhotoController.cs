using Microsoft.AspNetCore.Mvc;
using Models;
using PhotoApi.Services;

namespace PhotoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PhotoController : Controller
{
    private readonly ILogger<PhotoController> _logger;
    private readonly PhotoService _photoService;
    public PhotoController(ILogger<PhotoController> logger, PhotoService photoService)
    {
        _logger = logger;
        _photoService = photoService;
    }

    [HttpPost("get", Name = "GetPhoto")]
    public ActionResult<string> GetPhoto(PhotoRequest request)
    {
        
        return Ok("");
    }

    [HttpPost("refresh")]
    public IActionResult RefreshPhoto(List<PhotoList> list)
    {
        return Ok();
    }
}