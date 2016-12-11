using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Toponym.Core.Extensions;
using Toponym.Site.Extensions;
using Toponym.Site.Models;
using Group = Toponym.Site.Models.Group;

namespace Toponym.Site.Helpers {
    public class ItemHelper {

        public static List<Item> GetMached(IEnumerable<Item> allItems, Regex regex, Group type, Language language) {

            IEnumerable<Item> typedItems;

            switch (type) {

                case Group.All:
                    typedItems = allItems;
                    break;

                case Group.Populated:
                    typedItems = allItems.Where(i => i.Type.ToCategory() == Category.Populated);
                    break;

                case Group.Water:
                    typedItems = allItems.Where(i => i.Type.ToCategory() == Category.Water);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
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

        public static int GetMachedCount(IEnumerable<Item> allItems, Regex regex, Group type, Language language) {

            IEnumerable<Item> typedItems;

            switch (type) {

                case Group.All:
                    typedItems = allItems;
                    break;

                case Group.Populated:
                    typedItems = allItems.Where(i => i.Type.ToCategory() == Category.Populated);
                    break;

                case Group.Water:
                    typedItems = allItems.Where(i => i.Type.ToCategory() == Category.Water);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            switch (language) {
                case Language.Russian:
                    return typedItems.Count(i => regex.IsMatch(i.TitleRu.ToSimple()));

                case Language.Belarusian:
                    return typedItems.Count(i => i.TitleBe != null && regex.IsMatch(i.TitleBe.ToSimple()));

                case Language.English:
                    return typedItems.Count(i => regex.IsMatch(i.TitleEn.ToSimple()));

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }
    }
}
