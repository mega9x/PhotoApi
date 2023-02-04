using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace Models.Response
{
    public class SingleTraffic
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("referer")]
        public string Referer { get; set; }
        [JsonPropertyName("weight")]
        public string Weight { get; set; }
        [JsonPropertyName("short_url")]
        public string ShortUrl { get; set; }
        [JsonPropertyName("status")] 
        public string Status { get; set; }
        [JsonPropertyName("short_url_hits")] 
        public string ShortUrlHits { get; set; }
        [JsonPropertyName("group_url_hits")]
        public string GroupUrlHits { get; set; }
        [JsonPropertyName("total_hits")]
        public string TotalHits { get; set; }

        public SingleTraffic(HtmlNode row)
        {
            var allTd = row.QuerySelectorAll("td").ToList();
            Name = allTd[1].InnerText;
            Url = allTd[2].InnerText;
            Referer = allTd[3].InnerText;
            Weight = allTd[4].InnerText;
            ShortUrl = allTd[5].InnerText;
            Status = allTd[6].InnerText;
            ShortUrlHits = allTd[7].InnerText;
            GroupUrlHits = allTd[8].InnerText;
            TotalHits = allTd[9].InnerText;
        }
    }
}
