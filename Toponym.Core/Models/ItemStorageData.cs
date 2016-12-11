using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toponym.Core.Models {
    [DebuggerDisplay("{TitleRu} / {TitleBe}")]
    [JsonObject]
    public class ItemStorageData {

        [JsonProperty("ru")]
        public string TitleRu { get; set; }

        [JsonProperty("be", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleBe { get; set; }

        [JsonProperty("en")]
        public string TitleEn { get; set; }

        [JsonProperty("type")]
        public ItemType Type { get; set; }

        [JsonProperty("gps")]
        public float[] Gps { get; set; }

        [JsonProperty("screen")]
        public List<float[]> Screen { get; set; }

        public ItemStorageData() { }

        public ItemStorageData(string titleRu, string titleBe, string titleEn, ItemType type, GpsCoords gps, ScreenCoords screen) {
            if (titleRu == null)
                throw new ArgumentNullException(nameof(titleRu));
            //if (titleBe == null)
            //    throw new ArgumentNullException(nameof(titleBe));
            if (gps == null)
                throw new ArgumentNullException(nameof(gps));

            TitleRu = titleRu;
            TitleBe = titleBe;
            TitleEn = titleEn;
            Type = type;
            Gps = new[] { gps.Latitude, gps.Longitude };
            Screen = new List<float[]> { new[] { screen.X, screen.Y } };
        }
    }
}
