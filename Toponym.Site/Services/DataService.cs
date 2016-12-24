using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Toponym.Core.Models;
using Toponym.Core.Services;
using Toponym.Site.Models;
using Group = Toponym.Site.Models.Group;

namespace Toponym.Site.Services {
    public class DataService {

        private readonly IReadOnlyList<Item> _items;
        public static string CssBundleHash;
        public static string JsBundleHash;

        public DataService(IHostingEnvironment environment) {
            var dataPath = Path.Combine(environment.ContentRootPath, "App_Data", CoreConstants.DataFile);
            var data = FileService.Read<List<ItemStorageData>>(dataPath);
            _items = data.Select(i => new Item(i)).ToList();
            CssBundleHash = GetFileHash(Path.Combine(environment.WebRootPath, @"css\toponym.min.css"));
            JsBundleHash = GetFileHash(Path.Combine(environment.WebRootPath, @"js\toponym.min.js"));
        }

        public List<Item> GetItems(Regex regex, Group group, Language language) {

            IEnumerable<Item> groupItems;

            switch (group) {

                case Group.All:
                    groupItems = _items;
                    break;

                case Group.Populated:
                    groupItems = _items.Where(i => i.Category == Category.Populated);
                    break;

                case Group.Water:
                    groupItems = _items.Where(i => i.Category == Category.Water);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(group));
            }

            switch (language) {
                case Language.Russian:
                    return groupItems.Where(i => regex.IsMatch(i.TitleRuIndex)).ToList();

                case Language.Belarusian:
                    return groupItems.Where(i => i.TitleBe != null && regex.IsMatch(i.TitleBeIndex)).ToList();

                case Language.English:
                    return groupItems.Where(i => regex.IsMatch(i.TitleEn)).ToList();

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }

        private static string GetFileHash(string path) {
            using (var md5 = MD5.Create()) {
                using (var stream = File.OpenRead(path)) {
                    var hash = BitConverter.ToString(md5.ComputeHash(stream));
                    return hash.Replace("-", string.Empty).Substring(0, 16).ToLower();
                }
            }
        }
    }
}
