namespace Toponym.Web;

using System.Text.Json;
using System.Text.RegularExpressions;

public class DataService
{
    private readonly IReadOnlyList<Entry> _entries;
    public static string? ViteMainJs { get; private set; }
    public static string? ViteMainCss { get; private set; }

    public DataService(IWebHostEnvironment environment)
    {
        var dataPath = Path.Combine(environment.ContentRootPath, "App_Data", Constants.DataFileName);
        var dataJson = File.ReadAllText(dataPath);
        var data = NotNull(JsonSerializer.Deserialize<IReadOnlyList<EntryData>>(dataJson));
        _entries = data.Select(i => new Entry(i)).ToList();

        LoadViteManifest(environment.WebRootPath);
    }

    public IReadOnlyList<Entry> GetEntries(Regex regex, EntryCategory category, Language language)
    {
        var groupEntries = _entries.Where(i => (i.Category & category) != 0);

        return language switch
        {
            Language.Russian => groupEntries.Where(i => regex.IsMatch(i.TitleRuIndex)).ToList(),
            Language.Belarusian => groupEntries.Where(i => i.TitleBeIndex != null && regex.IsMatch(i.TitleBeIndex)).ToList(),
            Language.English => groupEntries.Where(i => regex.IsMatch(i.TitleEn)).ToList(),
            _ => throw new ArgumentOutOfRangeException(nameof(language)),
        };
    }

    private static void LoadViteManifest(string webRootPath)
    {
        var manifestPath = Path.Combine(webRootPath, "assets", "bundle", "manifest.json");
        if (!File.Exists(manifestPath))
            return;

        using var stream = File.OpenRead(manifestPath);
        using var doc = JsonDocument.Parse(stream);
        if (!doc.RootElement.TryGetProperty("src/main.tsx", out var entry))
            return;

        if (entry.TryGetProperty("file", out var file))
            ViteMainJs = file.GetString();

        if (entry.TryGetProperty("css", out var css) && css.ValueKind == JsonValueKind.Array && css.GetArrayLength() > 0)
            ViteMainCss = css[0].GetString();
    }
}
