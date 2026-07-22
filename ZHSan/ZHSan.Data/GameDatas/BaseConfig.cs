
using System.Text.Json.Serialization;

namespace GameDatas;

public class BaseConfig
{
    //JsonPropertyOrder仅用来序列化时将Id、Name排在最前

    [JsonPropertyOrder(-100)]
    public int Id { get; set; }

    [JsonPropertyOrder(-99)]
    public string Name { get; set; }
}