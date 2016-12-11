using Newtonsoft.Json;
using System.Collections.Generic;

namespace Toponym.Site.Models {
    [JsonObject]
    public class ResponseData {

        [JsonProperty("status")]
        public ResponseStatus Status { get; set; }

        [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
        public List<ItemData> Items { get; set; }

        [JsonProperty("matchCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? MatchCount { get; set; }
    }
}
