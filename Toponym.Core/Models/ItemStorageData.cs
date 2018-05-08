using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toponym.Core.Models
{
    [DebuggerDisplay("{TitleRu,nq} / {TitleBe ?? \"–\",nq}")]
    [JsonObject]
    public class ItemStorageData
    {
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
    }
}
