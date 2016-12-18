using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Toponym.Core.Extensions;
using Toponym.Core.Models;
using Toponym.Core.Services;
using Toponym.Site.Extensions;
using Toponym.Site.Models;
using Group = Toponym.Site.Models.Group;

namespace Toponym.Site.Services {
    public class DataService {

        private readonly IReadOnlyList<Item> _items;

        public DataService(IHostingEnvironment environment) {
            var dataDir = Path.Combine(environment.ContentRootPath, @"App_Data");
            var dataPath = Path.Combine(dataDir, CoreConstants.DataFile);
            var data = FileService.Read<List<ItemStorageData>>(dataPath);
            _items = data.Select(i => new Item(i)).ToList();
        }

        public List<Item> GetItems(Regex regex, Group type, Language language) {

            IEnumerable<Item> typedItems;

            switch (type) {

                case Group.All:
                    typedItems = _items;
                    break;

                case Group.Populated:
                    typedItems = _items.Where(i => i.Type.ToCategory() == Category.Populated);
                    break;

                case Group.Water:
                    typedItems = _items.Where(i => i.Type.ToCategory() == Category.Water);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            switch (language) {
                case Language.Russian:
                    return typedItems.Where(i => regex.IsMatch(i.TitleRu.ToSimple())).ToList();

                case Language.Belarusian:
                    return typedItems.Where(i => i.TitleBe != null && regex.IsMatch(i.TitleBe.ToSimple())).ToList();

                case Language.English:
                    return typedItems.Where(i => regex.IsMatch(i.TitleEn.ToSimple())).ToList();

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }
    }
}
