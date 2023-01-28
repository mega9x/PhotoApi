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
using Models.Response;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tomlyn;

//// if (!Directory.Exists(UpdaterConfig.ImageRoot))
//// {
////     Directory.CreateDirectory(UpdaterConfig.ImageRoot);
////     Console.WriteLine("基底文件夹不存在, 已经新建了一个");
////     Console.WriteLine(Path.GetFullPath(UpdaterConfig.ImageRoot));
////     Console.ReadLine();
////     return;
//// }
////
//// // 现在开始判断文件夹是否存在，如果不存在就创建
//// if(!Directory.Exists(UpdaterConfig.ConfigRoot))
//// {
////     Directory.CreateDirectory(UpdaterConfig.ConfigRoot);
////     Console.WriteLine("配置不存在, 已经新建了一个");
////     Console.WriteLine(Path.GetFullPath(UpdaterConfig.ConfigRoot)); // 把文件夹的整个路径打印出来, 不要让用户蒙在鼓里
//// }
//// // 所以现在来判断文件夹下面的配置文件是否存在，有一些情况就是有文件夹没文件的情况
//// // 这里用模板字符串来拼接字符串，这样就不用加号了
//// // 等等，我好像打错了
//// // 这样就对了
//// // 我现在把路径提取出来，方便一点
//// var configPath = $"{UpdaterConfig.ConfigRoot}/{UpdaterConfig.ConfigFile}";
//// if (!File.Exists(configPath))
//// {
////     // File.Create(configPath).Close(); 这行多余，因为 WriteAllText 函数会自动创建文件
////     // 然后，用 toml 的库来把“拥有默认值的对象”转换成字符串
////     var configString = Toml.FromModel(new UpdaterConfig());
////     // 最后，把字符串写入配置文件
////     File.WriteAllText(configPath, configString);
//// }
//// // 然后就是读取配置的部分. 首先把配置文件里的字符串读出来
//// var configStr = File.ReadAllText(configPath);
//// // 然后把字符串转换成类
//// var config = Toml.ToModel<UpdaterConfig>(configStr);
//// // 你结束了

// 获取配置
var config = Config.Instance.UpdaterConfig;
var client = new HttpClient()
{
    BaseAddress = new Uri(config.ApiRoot),
};
// 图片组列表
List<PhotoCategoryBucket> list = new();
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
        if (!imageList.Any()) continue;
        var photoList = new PhotoCategoryBucket
        {
            Name = key,
            Age = split[2],
            Gender = gender,
            Links = imageList.ToList(),
        };
        photoList.Name = c.Name;
        list.Add(photoList);
    }
}
else
{
    Console.WriteLine("未检测到图片样本文件, 即将从服务器获取样本");
    list = (await FetchSamples(client, config, 10)).ToList();
}

Console.WriteLine("读取成功. 开始爬取.");
var allPhotos = 0;
var driver = new ChromeDriver();

// 读取分组
foreach (var l in list)
{
    var originalList = l.Links;
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
                l.Links.Add(OpenButton[0].GetAttribute("href"));
                // imgUrl.Add(OpenButton[0].GetAttribute("href"));
                Console.WriteLine(OpenButton[0].GetAttribute("href"));
                var CloseButton = driver.FindElement(By.CssSelector("[class='MMViewerModal-Close']"));  //右上角灰色的关闭按钮元素
                CloseButton.Click();
            }
            catch
            {
                await Task.Delay(1000);
                continue;
            }
            Counts++;
        }
        allPhotos += Counts;
        Console.WriteLine($"本次成功爬取 {Counts} 张图片");
    }

    Console.WriteLine($"共爬取 {allPhotos} 张图片 上传中");
}

// var response = await ImageUpload(client, config, list);
// Console.WriteLine($"爬取完毕. 服务器响应: {await response.Content.ReadAsStringAsync()}");
Console.Read();
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

//给Yandex丢图片让他下载返回下好在Yandex服务器上图片的信息的诶皮埃
//https://yandex.com/images-apphost/image-download?url=
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
