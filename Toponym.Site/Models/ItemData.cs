using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Toponym.Core.Models;

namespace Toponym.Site.Models
{
    [JsonObject]
    public class ItemData
    {
        [JsonProperty("title")]
        public string Title { get; private set; }

        [JsonProperty("type")]
        public ItemType Type { get; private set; }

        [JsonProperty("gps")]
        public float[] Gps { get; private set; }

        [JsonProperty("screen")]
        public List<float[]> Screen { get; private set; }

        public ItemData(Item item, Language language)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            switch (language)
            {
                case Language.Russian:
                    Title = item.TitleRu;
                    break;

                case Language.Belarusian:
                    Title = item.TitleBe;
                    break;

                case Language.English:
                    Title = item.TitleEn;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }

            Type = item.Type;
            Gps = new[] { item.Gps.Latitude, item.Gps.Longitude };
            Screen = item.Screen.Select(i => new[] { i.X, i.Y }).ToList();
        }
    }
}
