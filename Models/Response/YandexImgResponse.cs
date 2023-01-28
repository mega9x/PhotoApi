using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Response
{
    public class YandexImgResponse
    {
        [JsonPropertyName("url")]
        public string Uri { get; set; }
    }
}
