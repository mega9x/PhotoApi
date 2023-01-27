using System.ComponentModel.Design;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ConstStr;
using Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Tomlyn;

// 抓图片并上传图片
// 这里叫做 Updater，所以我建了 UpdaterConfig 作为存常量的地方
if (!Directory.Exists(UpdaterConfig.IMAGE_ROOT))
{
    Directory.CreateDirectory(UpdaterConfig.IMAGE_ROOT);
    Console.WriteLine("基底文件夹不存在, 已经新建了一个");
    Console.WriteLine(Path.GetFullPath(UpdaterConfig.IMAGE_ROOT));
    Console.ReadLine();
    return;
}

// 现在开始判断文件夹是否存在，如果不存在就创建
if(!Directory.Exists(UpdaterConfig.CONFIG_ROOT))
{
    Directory.CreateDirectory(UpdaterConfig.CONFIG_ROOT);
    Console.WriteLine("配置不存在, 已经新建了一个");
    Console.WriteLine(Path.GetFullPath(UpdaterConfig.CONFIG_ROOT)); // 把文件夹的整个路径打印出来, 不要让用户蒙在鼓里
}
// 所以现在来判断文件夹下面的配置文件是否存在，有一些情况就是有文件夹没文件的情况
// 这里用模板字符串来拼接字符串，这样就不用加号了
// 等等，我好像打错了
// 这样就对了
// 我现在把路径提取出来，方便一点
var configPath = $"{UpdaterConfig.CONFIG_ROOT}/{UpdaterConfig.CONFIG_FILE}";
if (!File.Exists(configPath))
{
    // File.Create(configPath).Close(); 这行多余，因为 WriteAllText 函数会自动创建文件
    // 然后，用 toml 的库来把“拥有默认值的对象”转换成字符串
    var configString = Toml.FromModel(new UpdaterConfig());
    // 最后，把字符串写入配置文件
    File.WriteAllText(configPath, configString);
}
// 然后就是读取配置的部分. 首先把配置文件里的字符串读出来
var configStr = File.ReadAllText(configPath);
// 然后把字符串转换成类
var config = Toml.ToModel<UpdaterConfig>(configStr);
// 你结束了


// 图片组列表
List<PhotoList> list = new();
var dir = new DirectoryInfo(UpdaterConfig.IMAGE_ROOT);
var child = dir.GetFiles();
// 读取图片链接
foreach (var c in child)
{
    var split = c.Name.Split("-");
    var key = split[0];
    var gender = split[1];
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

// 读取分组
foreach (var l in list)
{
    var originalList = l.Links;

    foreach (var link in originalList)
    {
        driver.Navigate().GoToUrl("https://yandex.com/images/");
        await SearchImage(link, driver); // 直接在这里插入一个搜其他图的(直接访问链接就行了)
        driver.Navigate().GoToUrl("https://yandex.com/images/" + "&cbir_page=similar");
        int Counts = 0;   //记录已爬照片数
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
                    l.Links.Add(OpenButton[0].GetAttribute("href"));
                    // imgUrl.Add(OpenButton[0].GetAttribute("href"));
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

//https://yandex.com/images-apphost/image-download?url=
async Task SearchImage(string url, IWebDriver driver)
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
        Console.Write("无效的图片地址");
        return;
    }
    var ImgUrl = JsonSerializer.Deserialize<YandexImgUrl>(response);
    driver.Navigate().GoToUrl("https://yandex.com/images/search?family=yes&rpt=imageview&url=" + ImgUrl.url);
}
//图片上传
