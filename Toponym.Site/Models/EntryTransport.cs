using Newtonsoft.Json;
using System.Collections.Generic;

namespace Toponym.Site
{
    [JsonObject]
    public class EntryTransport
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public EntryType Type { get; set; }

        [JsonProperty("geo")]
        public float[] GeoPoint { get; set; }

        [JsonProperty("screen")]
        public List<float[]> ScreenPoints { get; set; }
    }
}
