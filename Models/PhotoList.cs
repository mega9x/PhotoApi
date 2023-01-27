using System;
using System.Dynamic;
using System.Text.Json.Serialization;

namespace Models;

public class PhotoList
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("gender")]
    public string Gender { get; set; }
    [JsonPropertyName("age")]
    public string Age { get; set; }
    [JsonPropertyName("images")]
    public List<string> Links { get; set; }
}