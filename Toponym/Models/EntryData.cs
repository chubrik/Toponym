using Newtonsoft.Json;
using System.Diagnostics;

namespace Toponym
{
    [DebuggerDisplay("{" + nameof(TitleRu) + ",nq} / {" + nameof(TitleBe) + " ?? \"–\",nq}")]
    [JsonObject]
    public class EntryData
    {
        [JsonProperty("ru")]
        public string TitleRu { get; }

        [JsonProperty("be", NullValueHandling = NullValueHandling.Ignore)]
        public string? TitleBe { get; }

        [JsonProperty("en")]
        public string TitleEn { get; }

        [JsonProperty("type")]
        public EntryType Type { get; }

        [JsonProperty("geo")]
        public float[] Location { get; private set; }

        [JsonProperty("screen")]
        public IReadOnlyList<float[]> ScreenPoints { get; private set; }

        public EntryData(
            string titleRu, string? titleBe, string titleEn, EntryType type, 
            float[] location, IReadOnlyList<float[]> screenPoints)
        {
            TitleRu = titleRu;
            TitleBe = titleBe;
            TitleEn = titleEn;
            Type = type;
            Location = location;
            ScreenPoints = screenPoints;
        }

        public void Relocate(float[] location, IReadOnlyList<float[]> screenPoints)
        {
            Location = location;
            ScreenPoints = screenPoints;
        }
    }
}
