using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Site.Extensions;

namespace Toponym.Site.Models
{
    [DebuggerDisplay("{" + nameof(TitleRu) + "}")]
    public class Entry
    {
        public string TitleRu { get; }
        public string TitleBe { get; }
        public string TitleEn { get; }
        public string TitleRuIndex { get; }
        public string TitleBeIndex { get; }
        public EntryType Type { get; }
        public GeoCoords Coords { get; }
        public List<ScreenCoords> Screen { get; }
        public Category Category { get; }

        public Entry(EntryData data)
        {
            Debug.Assert(data != null);

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            TitleRu = data.TitleRu;
            TitleBe = data.TitleBe;
            TitleEn = data.TitleEn;
            TitleRuIndex = data.TitleRu.ToLower().Replace('ё', 'е');

            if (data.TitleBe != null)
                TitleBeIndex = data.TitleBe.ToLower()
                    .Replace('ё', 'е')
                    .Replace('ў', 'у')
                    .Replace('і', 'i') // Cyrillic "i" to latin
                    .Replace('’', '\'');

            Type = data.Type;
            Category = data.Type.ToCategory();
            Coords = new GeoCoords(data.Coords[0], data.Coords[1]);
            Screen = data.Screen.Select(i => new ScreenCoords(i[0], i[1])).ToList();
        }
    }
}
