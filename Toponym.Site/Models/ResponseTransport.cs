using Newtonsoft.Json;
using System.Collections.Generic;

namespace Toponym.Site
{
    [JsonObject]
    public class ResponseTransport
    {
        [JsonProperty("status")]
        public ResponseStatus Status { get; set; }

        [JsonProperty("entries", NullValueHandling = NullValueHandling.Ignore)]
        public List<EntryTransport> Entries { get; set; }

        [JsonProperty("matchCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? MatchCount { get; set; }
    }
}
