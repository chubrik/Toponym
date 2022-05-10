namespace Toponym.Web;

using System.Diagnostics;

[DebuggerDisplay("{" + nameof(TitleRu) + "}")]
public class Entry
{
    public string TitleRu { get; }
    public string? TitleBe { get; }
    public string TitleEn { get; }
    public string TitleRuIndex { get; }
    public string? TitleBeIndex { get; }
    public EntryType Type { get; }
    public GeoPoint GeoPoint { get; }
    public IReadOnlyList<ScreenPoint> ScreenPoints { get; }
    public EntryCategory Category { get; }

    public Entry(EntryData data)
    {
        Debug.Assert(data != null);

        if (data == null)
            throw new ArgumentNullException(nameof(data));

        TitleRu = data.TitleRu;
        TitleBe = data.TitleBe;
        TitleEn = data.TitleEn;
        TitleRuIndex = data.TitleRu.ToLower().Replace('ё', 'е');

        if (data.TitleBe != null)
            TitleBeIndex = data.TitleBe.ToLower()
                .Replace('ё', 'е')
                .Replace('ў', 'у')
                .Replace('і', 'i') // Cyrillic "i" to latin
                .Replace('’', '\'');

        Type = data.Type;
        Category = data.Type.ToCategory();
        GeoPoint = new GeoPoint(data.Location[0], data.Location[1]);
        ScreenPoints = data.ScreenPoints.Select(i => new ScreenPoint(i[0], i[1])).ToList();
    }
}
