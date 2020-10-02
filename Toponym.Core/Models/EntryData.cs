using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toponym
{
    [DebuggerDisplay("{" + nameof(TitleRu) + ",nq} / {" + nameof(TitleBe) + " ?? \"–\",nq}")]
    [JsonObject]
    public class EntryData
    {
        [JsonProperty("ru")]
        public string TitleRu { get; set; }

        [JsonProperty("be", NullValueHandling = NullValueHandling.Ignore)]
        public string TitleBe { get; set; }

        [JsonProperty("en")]
        public string TitleEn { get; set; }

        [JsonProperty("type")]
        public EntryType Type { get; set; }

        [JsonProperty("geo")]
        public float[] GeoPoint { get; set; }

        [JsonProperty("screen")]
        public List<float[]> ScreenPoints { get; set; }
    }
}
