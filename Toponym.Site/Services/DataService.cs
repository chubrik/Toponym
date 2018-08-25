using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Toponym.Core.Models;
using Toponym.Site.Models;

namespace Toponym.Site.Services
{
    public class DataService
    {
        private readonly IReadOnlyList<Entry> _entries;
        public static string CssBundleHash;
        public static string JsBundleHash;

        public DataService(IHostingEnvironment environment)
        {
            var dataPath = Path.Combine(environment.ContentRootPath, "App_Data", Constants.DataFileName);

            using (var fileStream = File.OpenRead(dataPath))
            using (var streamReader = new StreamReader(fileStream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var data = new JsonSerializer().Deserialize<List<EntryData>>(jsonTextReader);
                _entries = data.Select(i => new Entry(i)).ToList();
            }

            CssBundleHash = GetFileHash(Path.Combine(environment.WebRootPath, @"assets\css\toponym.min.css"));
            JsBundleHash = GetFileHash(Path.Combine(environment.WebRootPath, @"assets\js\toponym.min.js"));
        }

        public List<Entry> GetEntries(Regex regex, EntryCategory category, Language language)
        {
            var groupEntries = _entries.Where(i => (i.Category & category) != 0);

            switch (language)
            {
                case Language.Russian:
                    return groupEntries.Where(i => regex.IsMatch(i.TitleRuIndex)).ToList();

                case Language.Belarusian:
                    return groupEntries.Where(i => i.TitleBe != null && regex.IsMatch(i.TitleBeIndex)).ToList();

                case Language.English:
                    return groupEntries.Where(i => regex.IsMatch(i.TitleEn)).ToList();

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }

        private static string GetFileHash(string path)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path))
            {
                var hash = BitConverter.ToString(md5.ComputeHash(stream));
                return hash.Replace("-", string.Empty).Substring(0, 16).ToLower();
            }
        }
    }
}
