using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Request;
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
    public async Task<ActionResult<IEnumerable<string>>> GetPhoto(PhotoRequest request)
    {
        var photos = await _photoService.GetPhotos(request);
        if (!photos.Any())
        {
            return BadRequest("没有找到与查询匹配的图片");
        }
        return Ok(photos);
    }
    // 获取图片
    [HttpPost("GetPhotoGroup")]
    public async Task<ActionResult<IEnumerable<string>>> GetPhotoGroup(PhotoRequest request)
    {
        var photos = await _photoService.GetPhotoGroup(request);
        if (!photos.Any())
        {
            return BadRequest("没有找到与查询匹配的图片");
        }
        return Ok(photos);
    }
    // 随机获取图片
    [HttpGet("GetRandomPhoto")]
    public async Task<ActionResult<IEnumerable<string>>> GetPhoto(int num)
    {
        var photos = await _photoService.GetPhotosRandomly(num);
        if (!photos.Any())
        {
            return BadRequest("没有找到与查询匹配的图片");
        }
        return Ok(photos);
    }
    // 上传图片桶
    [HttpPost("UploadByBucket")]
    public IActionResult UploadByBucket(PhotoBucketRequest request)
    {
        _photoService.AddBucketOneByOne(request.Bucket);
        return Ok();
    }
    // 获取所有类别的随机采样
    [HttpGet("GetSamples")]
    public ActionResult<IEnumerable<PhotoCategoryBucket>> GetSamples(int eachNum)
    {
        var num = eachNum;
        if (eachNum > 20)
        {
            num = 20;
        }
        return Ok(_photoService.BuildNewSamples(num));
    }
}