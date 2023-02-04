using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConstStr.Api;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Models;
using Models.Response;

namespace Crawler
{
    public class LlinksCrawler
    {
        private string _sessionId = "";
        private int maxTime = 120;
        private int requestTime = 120;
        private HttpClient _client = new();
        private string _account = "";
        private string _password = "";
        public LlinksCrawler(string account, string password)
        {
            _account = account;
            _password = password;
            _client.DefaultRequestHeaders.Add("user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36 Edg/109.0.1518.70");
        }

        public async Task Login()
        {
            requestTime++;
            if (requestTime < maxTime)
            {
                return;
            }
            requestTime = 0;
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler()
            {
                CookieContainer = cookieContainer,
            };
            _client = new(handler)
            {
                BaseAddress = new Uri(Llinks.Root),
            };
            var response = await _client.GetAsync(Llinks.BaseEndpoint);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(await response.Content.ReadAsStringAsync());
            var token = htmlDocument.DocumentNode.QuerySelector("#member_login > input[type=hidden]")
                .GetAttributeValue("value", "sG3CkNx8fRHXAvwt");
            await _client.PostAsync(Llinks.LoginEndpoint, new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"_token", token},
                {"email", _account},
                {"password", _password},
                {"login", ""}
            }));
        }
        public async Task<List<SingleTraffic>?> GetTraffics(string id)
        {
            await Login();
            var timeout = 0;
            while (timeout <= 30)
            {
                try
                {
                    var doc = await _client.GetAsync($"{Llinks.GroupDetailsEndpoint}{id}");
                    var str = await doc.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(str);
                    var rootNode = htmlDocument.DocumentNode;
                    var allRow = rootNode.QuerySelectorAll(".selectable-row");
                    return allRow.Select(row => new SingleTraffic(row)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    timeout++;
                }
            }
            return null;
        }

        public async Task<List<GroupTraffic>> GetAllTraffics()
        {
            await Login();
            var timeout = 0;
            var allList = new List<GroupTraffic>();
            while (timeout <= 30)
            {
                try
                {
                    var doc = await _client.GetAsync($"{Llinks.GroupsTableEndpoint}");
                    var str = await doc.Content.ReadAsStringAsync();
                    var htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(str);
                    var rootNode = htmlDocument.DocumentNode;
                    var allRow = rootNode.QuerySelectorAll(".selectable-row");
                    foreach (var row in allRow)
                    {
                        var tds = row.QuerySelectorAll("td").ToArray();
                        if (!tds[3].InnerText.Contains("Standard")) continue;
                        allList.Add(new GroupTraffic(tds[1].InnerText,row.GetAttributeValue("data-group-id", "0")  ,await GetTraffics(row.GetAttributeValue("data-group-id", ""))));
                    }
                    return allList;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    timeout++;
                }
            }
            return null;
        }
    }
}
