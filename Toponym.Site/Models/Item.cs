using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Site.Extensions;

namespace Toponym.Site.Models {
    [DebuggerDisplay("{TitleRu}")]
    public class Item {

        public string TitleRu { get; set; }
        public string TitleBe { get; set; }
        public string TitleEn { get; set; }
        public ItemType Type { get; set; }
        public GpsCoords Gps { get; set; }
        public List<ScreenCoords> Screen { get; set; }

        public Category Category => Type.ToCategory();

        public Item(ItemStorageData data) {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            TitleRu = data.TitleRu;
            TitleBe = data.TitleBe;
            TitleEn = data.TitleEn;
            Type = data.Type;
            Gps = new GpsCoords(data.Gps[0], data.Gps[1]);
            Screen = data.Screen.Select(i => new ScreenCoords(i[0], i[1])).ToList();
        }
    }
}
