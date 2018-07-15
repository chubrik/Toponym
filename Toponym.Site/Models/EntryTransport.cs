using Newtonsoft.Json;
using System.Collections.Generic;
using Toponym.Core.Models;

namespace Toponym.Site.Models
{
    [JsonObject]
    public class EntryTransport
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("type")]
        public EntryType Type { get; set; }

        [JsonProperty("geo")]
        public float[] Coords { get; set; }

        [JsonProperty("screen")]
        public List<float[]> Screen { get; set; }
    }
}
