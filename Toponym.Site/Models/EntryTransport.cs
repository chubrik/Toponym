﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Toponym.Site
{
    public class EntryTransport
    {
        public string Title { get; set; }

        public EntryType Type { get; set; }

        [JsonPropertyName("geo")]
        public float[] GeoPoint { get; set; }

        [JsonPropertyName("screen")]
        public List<float[]> ScreenPoints { get; set; }
    }
}
