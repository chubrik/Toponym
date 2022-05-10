namespace Toponym.Tools;

using OsmDataKit;

public static class GeoObjectExtentions
{
    private const string TitleRuKey = "_titleRu";
    private const string TitleBeKey = "_titleBe";

    public static string? TitleRu(this GeoObject geo) =>
        geo.Tags?.GetValueOrDefault(TitleRuKey);

    public static string? TitleBe(this GeoObject geo) =>
        geo.Tags?.GetValueOrDefault(TitleBeKey);

    public static void SetTitleRu(this GeoObject geo, string value)
    {
        if (geo.Tags == null)
            geo.Tags = new Dictionary<string, string>();

        geo.Tags[TitleRuKey] = value;
    }

    public static void SetTitleBe(this GeoObject geo, string value)
    {
        if (geo.Tags == null)
            geo.Tags = new Dictionary<string, string>();

        geo.Tags[TitleBeKey] = value;
    }

    public static EntryData ToEntryDataAsPoint(this GeoObject geo, EntryType type) =>
        EntryHelper.GetData(NotNull(geo.TitleRu()), geo.TitleBe(), type, geo.CenterLocation());
}
