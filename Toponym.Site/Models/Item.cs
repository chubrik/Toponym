using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Site.Extensions;

namespace Toponym.Site.Models
{
    [DebuggerDisplay("{" + nameof(TitleRu) + "}")]
    public class Item
    {
        public string TitleRu { get; private set; }
        public string TitleBe { get; private set; }
        public string TitleEn { get; private set; }
        public string TitleRuIndex { get; private set; }
        public string TitleBeIndex { get; private set; }
        public ItemType Type { get; private set; }
        public GpsCoords Gps { get; private set; }
        public List<ScreenCoords> Screen { get; private set; }
        public Category Category { get; private set; }

        public Item(ItemStorageData data)
        {
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
            Gps = new GpsCoords(data.Gps[0], data.Gps[1]);
            Screen = data.Screen.Select(i => new ScreenCoords(i[0], i[1])).ToList();
        }
    }
}
