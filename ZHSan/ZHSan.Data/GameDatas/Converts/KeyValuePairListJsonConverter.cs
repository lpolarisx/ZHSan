namespace GameDatas.Converts;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 以 [{"Key":k,"Value":v}] 形式读写 List&lt;KeyValuePair&lt;int,int&gt;&gt;，允许重复 Key。
/// 读取时兼容旧存档的字典格式 {"k":v}。
/// </summary>
public class KeyValuePairListJsonConverter : JsonConverter<List<KeyValuePair<int, int>>>
{
    public override List<KeyValuePair<int, int>> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new List<KeyValuePair<int, int>>();

        switch (reader.TokenType)
        {
            case JsonTokenType.Null:
                return result;

            // 旧格式：Dictionary<int,int>
            case JsonTokenType.StartObject:
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    int key = int.Parse(reader.GetString());
                    reader.Read();
                    result.Add(new KeyValuePair<int, int>(key, reader.GetInt32()));
                }
                return result;

            case JsonTokenType.StartArray:
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType != JsonTokenType.StartObject)
                        throw new JsonException();

                    int key = 0;
                    int value = 0;

                    while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                    {
                        string propertyName = reader.GetString();
                        reader.Read();

                        if (string.Equals(propertyName, "Key", StringComparison.OrdinalIgnoreCase))
                            key = reader.GetInt32();
                        else if (string.Equals(propertyName, "Value", StringComparison.OrdinalIgnoreCase))
                            value = reader.GetInt32();
                        else
                            reader.Skip();
                    }

                    result.Add(new KeyValuePair<int, int>(key, value));
                }
                return result;

            default:
                throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, List<KeyValuePair<int, int>> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var pair in value)
        {
            writer.WriteStartObject();
            writer.WriteNumber("Key", pair.Key);
            writer.WriteNumber("Value", pair.Value);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }
}
