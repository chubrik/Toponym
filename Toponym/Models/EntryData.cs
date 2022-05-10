namespace Toponym;

using System.Diagnostics;
using System.Text.Json.Serialization;

[DebuggerDisplay("{" + nameof(TitleRu) + ",nq} / {" + nameof(TitleBe) + " ?? \"–\",nq}")]
public class EntryData
{
    [JsonPropertyName("ru")]
    public string TitleRu { get; }

    [JsonPropertyName("be")]
    public string? TitleBe { get; }

    [JsonPropertyName("en")]
    public string TitleEn { get; }

    [JsonPropertyName("type")]
    public EntryType Type { get; }

    [JsonPropertyName("geo")]
    public float[] Location { get; private set; }

    [JsonPropertyName("screen")]
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
