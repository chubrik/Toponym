using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Toponym.Web
{
    public class DataService
    {
        private readonly IReadOnlyList<Entry> _entries;
        public static string? CssBundleHash { get; private set; }
        public static string? JsBundleHash { get; private set; }

        public DataService(IWebHostEnvironment environment)
        {
            var dataPath = Path.Combine(environment.ContentRootPath, "App_Data", Constants.DataFileName);
            var dataJson = File.ReadAllText(dataPath);
            var data = NotNull(JsonSerializer.Deserialize<IReadOnlyList<EntryData>>(dataJson));
            _entries = data.Select(i => new Entry(i)).ToList();

            CssBundleHash = GetFileHash(Path.Combine(environment.WebRootPath, @"assets\css\toponym.min.css"));
            JsBundleHash = GetFileHash(Path.Combine(environment.WebRootPath, @"assets\js\toponym.min.js"));
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

        private static string GetFileHash(string path)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(path);
            var hash = BitConverter.ToString(md5.ComputeHash(stream));
            return hash.Replace("-", string.Empty)[..16].ToLower();
        }
    }
}
