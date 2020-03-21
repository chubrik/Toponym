using System.Collections.Generic;
using System.Linq;

namespace Toponym.Tools
{
    public static class EntryDataExtensions
    {
        public static List<EntryData> ToSortedList(this IEnumerable<EntryData> entries) =>
            entries.OrderBy(i => i.TitleRu.ToLower().Replace("ё", "е").Replace(" ", "!"))
                   .ThenByDescending(i => i.Location[0]).ThenBy(i => i.Location[1]).ToList();
    }
}
