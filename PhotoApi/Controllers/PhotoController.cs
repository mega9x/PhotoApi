using System.Reflection.Metadata.Ecma335;
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
    // 获取图片
    [HttpPost("GetPhoto")]
    public ActionResult<IEnumerable<string>> GetPhoto(PhotoRequest request)
    {
        var photos = _photoService.GetPhotos(request);
        if (!photos.Any())
        {
            return BadRequest("没有找到与查询匹配的图片");
        }
        return Ok(photos);
    }
    // 上传图片
    [HttpPost("Upload")]
    public IActionResult UploadPhotos(List<PhotoList> list)
    {
        return Ok();
    }
}