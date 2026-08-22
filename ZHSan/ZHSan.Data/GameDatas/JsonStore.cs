using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using GameDatas.Converts;

namespace GameDatas;

public class JsonStore<T>
{
    private readonly string _filePath;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new PointJsonConverter() } 
    };

    public JsonStore(string filePath) => _filePath = filePath;

    public async Task<List<T>> LoadAsync()
    {
        if (!File.Exists(_filePath)) return new List<T>();

        await using var stream = File.OpenRead(_filePath);

        return await JsonSerializer.DeserializeAsync<List<T>>(stream, _options) ?? new List<T>();
    }

    public async Task SaveAsync(List<T> data)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath));

        await using var stream = File.Create(_filePath);

        await JsonSerializer.SerializeAsync(stream, data, _options);
    }

    public List<T> Load()
    {
        if (!File.Exists(_filePath)) return new List<T>();

        using var stream = File.OpenRead(_filePath);

        return JsonSerializer.Deserialize<List<T>>(stream, _options) ?? new List<T>();
    }

    public void Save(List<T> data)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath));

            using var stream = File.Create(_filePath);

            JsonSerializer.Serialize(stream, data, _options);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
