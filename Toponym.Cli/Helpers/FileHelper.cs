namespace Toponym.Cli;

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class FileHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static T ReadData<T>(string path) where T : class
    {
        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<T>(json, _options);
        return NotNull(data);
    }

    public static void WriteData<T>(string path, T data) where T : class
    {
        var json = JsonSerializer.Serialize(data, _options);
        var dir = NotNull(Path.GetDirectoryName(path));

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        File.WriteAllText(path, json);
    }
}
