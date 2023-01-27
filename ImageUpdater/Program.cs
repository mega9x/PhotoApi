using System.ComponentModel.Design;
// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ConstStr;
using Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

// 抓图片并上传图片

if (!Directory.Exists(FontendConfig.IMAGE_ROOT))
{
    Directory.CreateDirectory(FontendConfig.IMAGE_ROOT);
    Console.WriteLine("基底文件夹不存在, 已经新建了一个");
    Console.WriteLine(Path.GetFullPath(FontendConfig.IMAGE_ROOT));
    Console.ReadLine();
    return;
}
List<PhotoList> list = new();
var dir = new DirectoryInfo(FontendConfig.IMAGE_ROOT);
var child = dir.GetFiles();
// 读取图片链接
foreach (var c in child)
{
    var split = c.Name.Split("-");
    var key = split[0];
    var gender = split[1] == "男" ? 0 : 1;
    var age = split[2];
    var imageList = File.ReadAllLines(c.FullName);
    var photoList = new PhotoList
    {
        Name = key,
        Age = split[2],
        Gender = gender,
        Links = imageList.ToList(),
    };
    photoList.Name = c.Name;
    list.Add(photoList);
}

var driver = new ChromeDriver();
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);

var _ = driver.Url;

// 开爬
foreach (var l in list)
{
    var imgUrl = new List<string>();
    driver.Navigate().GoToUrl("https://yandex.com/images/");
    await ClickCameraButton();
    foreach (var link in l.Links)
    {
        await SearchImage(link);
        await ClickCameraButton();
        while (true)
        {
            var Image = driver.FindElements(By.CssSelector("[class='serp-item__thumb justifier__thumb']"));
            if (Image.Count == 0)
            {
                await Task.Delay(1000);
                continue;
            }
            Console.WriteLine("[" + System.DateTime.Now + "]" + "当前页面共 " + Image.Count + "张图片");
            for (int t = 0; t < Image.Count;)
            {
                try
                {
                    Image[t].Click();
                    var OpenButton = driver.FindElements(By.CssSelector("[class='Button2 Button2_size_m Button2_type_link Button2_view_action Button2_width_max MMViewerButtons-OpenImage']"));
                    if (OpenButton.Count == 0)
                    {
                        OpenButton = driver.FindElements(By.CssSelector("[class='Button2 Button2_size_m Button2_type_link Button2_view_default Button2_width_max MMViewerButtons-OpenImage']"));
                    }
                    imgUrl.Add(OpenButton[0].GetAttribute("href"));
                    Console.WriteLine(OpenButton[0].GetAttribute("href"));
                    var CloseButton = driver.FindElement(By.CssSelector("[class='MMViewerModal-Close']"));
                    CloseButton.Click();
                    t++;
                    Counts++;
                }
                catch
                {
                    await Task.Delay(1000);
                    continue;
                }
            }
            break;
        }
    }
}
async Task LoadImgsUrls()
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
}


async Task ClickCameraButton()
{
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
}
async Task SearchImage(string url)
{
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
            Thread.Sleep(3000);
            break;
        }
    }
}
static async Task<string> ImageToBase64ImageFormat(string uri)
{
    using var client = new HttpClient();
    var bytes = await client.GetByteArrayAsync(uri);
    return "image/jpeg;base64," + Convert.ToBase64String(bytes);
}