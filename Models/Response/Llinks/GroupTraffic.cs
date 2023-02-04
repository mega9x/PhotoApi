using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Models.Response;

namespace Models
{
    public class GroupTraffic
    {
        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("trafficGroup")]
        public List<SingleTraffic>? TrafficGroup { get; set; }

        public GroupTraffic(string groupName,string id, List<SingleTraffic>? trafficGroup)
        {
            Id = id;
            GroupName = groupName;
            TrafficGroup = trafficGroup;
        }
    }
}
