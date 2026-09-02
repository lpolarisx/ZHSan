#nullable enable

using System;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using GameDatas.Converts;

namespace GameDatas;

public sealed class GameDataArchive : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly ZipArchive _archive;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters =
        {
            new PointJsonConverter()
        }
    };

    private GameDataArchive(FileStream fileStream, ZipArchive archive)
    {
        _fileStream = fileStream;
        _archive = archive;
    }

    public static GameDataArchive Open(string filePath)
    {
        var directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

        var archive = new ZipArchive(stream, ZipArchiveMode.Update);

        return new GameDataArchive(stream, archive);
    }

    public T? Load<T>(string entryName)
    {
        var entry = _archive.GetEntry(entryName);

        if (entry == null)
        {
            return default;
        }

        using var stream = entry.Open();

        return JsonSerializer.Deserialize<T>(stream, _options);
    }

    public async Task<T?> LoadAsync<T>(string entryName)
    {
        var entry = _archive.GetEntry(entryName);

        if (entry == null)
        {
            return default;
        }

        await using var stream = entry.Open();

        return await JsonSerializer.DeserializeAsync<T>(stream, _options);
    }

    public void Save<T>(string entryName, T data)
    {
        Delete(entryName);

        var entry = _archive.CreateEntry(entryName, CompressionLevel.Fastest);

        using var stream = entry.Open();

        JsonSerializer.Serialize(stream, data, _options);
    }

    public async Task SaveAsync<T>(string entryName, T data)
    {
        Delete(entryName);

        var entry = _archive.CreateEntry(entryName, CompressionLevel.Fastest);
        
        await using var stream = entry.Open();

        await JsonSerializer.SerializeAsync(stream, data, _options);
    }

    public bool Exists(string entryName)
    {
        return _archive.GetEntry(entryName) != null;
    }

    public void Delete(string entryName)
    {
        _archive.GetEntry(entryName)?.Delete();
    }

    public void Dispose()
    {
        _archive.Dispose();
        _fileStream.Dispose();
    }
}