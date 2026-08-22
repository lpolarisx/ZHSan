
namespace GameDatas.Converts;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

public class PointJsonConverter : JsonConverter<Point>
{
    public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        int x = 0;
        int y = 0;

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return new Point(x, y);

            if (reader.TokenType != JsonTokenType.PropertyName)
                continue;

            string propertyName = reader.GetString();

            reader.Read();

            switch (propertyName)
            {
                case "X":
                case "x":
                    x = reader.GetInt32();
                    break;

                case "Y":
                case "y":
                    y = reader.GetInt32();
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);

        writer.WriteEndObject();
    }
}