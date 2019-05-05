using System.Collections.Generic;
using System.Linq;
using Toponym.Core.Models;

namespace Toponym.Tools.Extensions
{
    public static class EntryDataExtensions
    {
        public static List<EntryData> ToSortedList(this IEnumerable<EntryData> entries) =>
            entries.OrderBy(i => i.TitleRu.ToLower().Replace("ё", "е").Replace(" ", "!"))
                   .ThenByDescending(i => i.Coords[0]).ThenBy(i => i.Coords[1]).ToList();
    }
}
