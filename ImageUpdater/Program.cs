using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Net.Http.Json;
using System.Text.Json;
using ConstStr;
using ImageUpdater;
using Models;
using Models.Request;
using Models.Response;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tomlyn;

// 获取配置
var config = Config.Instance.UpdaterConfig;
var client = new HttpClient()
{
    BaseAddress = new Uri(config.ApiRoot),
};
// 图片组列表
var bucketList = (await FetchSamples(client, config, 10)).ToList();
var dir = new DirectoryInfo(UpdaterConfig.ImageRoot);
var child = dir.GetFiles();
if (child.Any())
{
    // 读取图片链接
    foreach (var c in child)
    {
        var split = c.Name.Split("-");
        var key = split[0];
        var gender = split[1];
        var age = split[2];
        var imageList = File.ReadAllLines(c.FullName);
        // 合并同类项
        if (!imageList.Any()) continue;
        var found = bucketList.First(x => x.Age == age && x.Gender == gender && x.Name == key);
        if (found is not null)
        {
            found.Links.AddRange(imageList);
        }
        else
        {
            var photoList = new PhotoCategoryBucket
            {
                Name = key,
                Age = split[2],
                Gender = gender,
                Links = imageList.ToList(),
            };
            photoList.Name = key;
            bucketList.Add(photoList);
        }
    }
}

Console.WriteLine("读取成功. 开始爬取.");
var photosCount = 0;
var driver = new ChromeDriver();

// 读取分组
foreach (var bucket in bucketList)
{
    var originalList = new List<string>(bucket.Links);
    foreach (var link in originalList)
    {
        var areYouOk = await SearchImage(link, driver);
        if (!areYouOk)
        {
            continue;
        }
        IReadOnlyCollection<IWebElement> images;
        int Counts = 0;   //记录已爬照片数
        while (true)
        {
            images = driver.FindElements(By.CssSelector("[class='serp-item__thumb justifier__thumb']"));  // 图片元素
            if (images.Count == 0)
            {
                await Task.Delay(1000);
                continue;
            }
            break;
        }
        Console.WriteLine("[" + System.DateTime.Now + "]" + "当前页面共 " + images.Count + "张图片");
        foreach (var element in images)
        {
            try
            {
                images = driver.FindElements(By.CssSelector("[class='serp-item__thumb justifier__thumb']"));  // 图片元素 
                var MoreImageButton = driver.FindElements(By.CssSelector("[class='button2 button2_size_l button2_theme_action button2_type_link button2_view_classic more__button i-bem']"));  //More images按钮元素
                if (MoreImageButton.Count == 1)
                {
                    MoreImageButton[0].Click();
                }
                element.Click();
                var OpenButton = driver.FindElements(By.CssSelector("[class='Button2 Button2_size_m Button2_type_link Button2_view_action Button2_width_max MMViewerButtons-OpenImage']"));  //黄色的Open Button按钮元素
                if (OpenButton.Count == 0)
                {
                    OpenButton = driver.FindElements(By.CssSelector("[class='Button2 Button2_size_m Button2_type_link Button2_view_default Button2_width_max MMViewerButtons-OpenImage']"));  //灰色的Oen Button按钮元素
                }
                bucket.Links.Add(OpenButton[0].GetAttribute("href"));
                Console.WriteLine(OpenButton[0].GetAttribute("href"));
                var CloseButton = driver.FindElement(By.CssSelector("[class='MMViewerModal-Close']"));  //右上角灰色的关闭按钮元素
                CloseButton.Click();
            }
            catch
            {
                await Task.Delay(500);
                continue;
            }
            Counts++;
        }
        photosCount += Counts;
        Console.WriteLine($"本次成功爬取 {Counts} 张图片");
    }
    var uploadRequest = new PhotoBucketRequest()
    {
        Bucket = bucket,
        IsContinue = true
    };
    if (bucketList.Last().Equals(bucket))
    {
        uploadRequest.IsContinue = false;
    }
    // 上传 bucket
    Console.WriteLine($"目前共爬取 {photosCount} 张图片 上传中");
    var response = await client.PostAsJsonAsync(config.UploadBucketEndpoint, uploadRequest);
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"发生未知错误于: {bucket.Name}-{bucket.Gender}-{bucket.Age}");
        Console.WriteLine(response.StatusCode);
    }
}
Console.WriteLine($"共爬取 {photosCount} 张图片");

// var response = await ImageUpload(client, config, list);
// Console.WriteLine($"爬取完毕. 服务器响应: {await response.Content.ReadAsStringAsync()}");
Console.Read();

async Task<bool> SearchImage(string url, IWebDriver driver)
{
    while (driver.Url.Contains("showcaptcha"))
    {
        await Task.Delay(1000);
    }
    var client = new HttpClient();
    string response;
    try
    {
        response = await client.GetStringAsync("https://yandex.com/images-apphost/image-download?url=" + url);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.Write("无效的图片地址. 正在跳过...");
        Console.WriteLine($"地址: {url}");
        return false;
    }
    var ImgUrl = JsonSerializer.Deserialize<YandexImgResponse>(response);
    driver.Navigate().GoToUrl($"https://yandex.com/images/search?family=yes&rpt=imageview&url={ImgUrl.Uri}");
    await Task.Delay(1000);
    driver.Navigate().GoToUrl($"{driver.Url}&cbir_page=similar");
    return true;
}
// 图片上传
async Task<HttpResponseMessage> ImageUpload(HttpClient client, UpdaterConfig config, IEnumerable<PhotoCategoryBucket> list)
{
    var response = await client.PostAsJsonAsync<IEnumerable<PhotoCategoryBucket>>(config.UploadEndpoint, list);
    return response;
}

// 样本获取 
async Task<IEnumerable<PhotoCategoryBucket>> FetchSamples(HttpClient client, UpdaterConfig config, int num)
{
    var response = await client.GetFromJsonAsync<IEnumerable<PhotoCategoryBucket>>($"{config.FetchSamplesEndpoint}?eachNum={num}");
    return response ?? new List<PhotoCategoryBucket>();
}
// 是否继续在上面就判断了，只需要判断是不是最后一个就可以，这个继续不继续已经包含在请求里面了
// 不用写 is continue, 直接传就行了，传PhotoBucketRequest
//async Task<bool> IsContinue (IEnumerable<PhotoCategoryBucket> list ,HttpClient client , UpdaterConfig config)
//{
//    var response = await client.PostAsJsonAsync<IEnumerable<PhotoCategoryBucket>>(config.UploadEndpoint, list);
//}

/*async Task LoadImgsUrls()
{
    while (true)
    {
        var counts = 0;
        var Image = driver.FindElements(By.CssSelector("[class='serp-item__thumb justifier__thumb']"));
        if (Image.Count == 0)
        {
            await Task.Delay(1000);
            continue;
        }
        Console.WriteLine("[" + System.DateTime.Now + "]" + "当前页面共 " + Image.Count + "张图片");
        for (int t = counts; t < Image.Count;)
        {
            try
            {
                Image[counts].Click();
                var OpenButton = driver.FindElement(By.CssSelector("[class='Button2 Button2_size_m Button2_type_link Button2_view_action Button2_width_max MMViewerButtons-OpenImage']"));
                ImageUrls.Add(OpenButton.GetAttribute("href"));
                Console.WriteLine(OpenButton.GetAttribute("href"));
                var CloseButton = driver.FindElement(By.CssSelector("[class='MMViewerModal-Close']"));
                CloseButton.Click();
                t++;
                counts++;
            }
            catch
            {
                await Task.Delay(1000);
                continue;
            }
        }
    }
}*/

//旧的点击方式搜索图片
/*async Task SearchImage(string url)
{
    //点击Yandex主页的镜头按钮
    while (true)
    {
        try
        {
            var CameraButton = driver.FindElement(By.CssSelector("[class='cbir-icon input__cbir-button-icon']"));
            if (CameraButton == null)
            {
                await Task.Delay(1000);
                continue;
            }
            CameraButton.Click();
            break;
        }
        catch (Exception e)
        {
            continue;
        }
    }
    //输入图片地址并确定
    while (true)
    {
        try
        {
            var SearchText = driver.FindElement(By.CssSelector("[class='Textinput-Control']"));
            SearchText.SendKeys(url);
            driver.FindElement(By.CssSelector("[class='Button2 Button2_view_default Button2_size_m Button2_width_auto CbirPanel-UrlFormButton']")).Click();
            await Task.Delay(3000);
            var RightOrNot = driver.FindElements(By.CssSelector("[class='Icon Icon_size_l Icon_type_error-filled CbirUploadErrorNotify-IconError']"));
            if (RightOrNot.Count == 1)
            {
                throw new Exception();
            }
            break;
        }
        catch (Exception e)
        {
            Console.WriteLine($"错误的URL地址: {url}");
            break;
        }
    }
}
static async Task<string> ImageToBase64ImageFormat(string uri)
{
    using var client = new HttpClient();
    var bytes = await client.GetByteArrayAsync(uri);
    return "image/jpeg;base64," + Convert.ToBase64String(bytes);
}*/