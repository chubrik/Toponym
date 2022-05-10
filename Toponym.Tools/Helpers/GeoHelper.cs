namespace Toponym.Tools;

using OsmDataKit;
using OsmSharp;
using System.Diagnostics;
using System.Text.RegularExpressions;

public static class GeoHelper
{
    private static readonly IReadOnlyList<string> _tagRuNames = new[] { "name", "int_name", "alt_name" };

    public static string? TitleRu(OsmGeo geo)
    {
        Debug.Assert(geo != null);

        if (geo == null)
            throw new ArgumentNullException(nameof(geo));

        return TitleRuBase(geo.Tags.ToDictionary(i => i.Key, i => i.Value));
    }

    public static string? TitleRu(GeoObject geo)
    {
        Debug.Assert(geo != null);

        if (geo == null)
            throw new ArgumentNullException(nameof(geo));

        return TitleRuBase(geo.Tags);
    }

    private static string? TitleRuBase(IReadOnlyDictionary<string, string>? tags)
    {
        if (tags == null || tags.Count == 0)
            return null;

        if (tags.TryGetValue("name:ru", out string? title) && !string.IsNullOrEmpty(title))
            return title;

        foreach (var tagName in _tagRuNames)
            if (tags.TryGetValue(tagName, out title) && !string.IsNullOrEmpty(title))
            {
                var lower = title.ToLower();

                if (!Regex.IsMatch(lower, "[a-zіў]") &&
                    !lower.Contains("чы") &&
                    !lower.Contains("ць") &&
                    !lower.Contains("ця"))
                    return title;
            }

        return null;
    }

    public static string? TitleBe(GeoObject geo)
    {
        Debug.Assert(geo != null);

        if (geo == null)
            throw new ArgumentNullException(nameof(geo));

        if (geo.Tags != null && geo.Tags.TryGetValue("name:be", out string? title) && !string.IsNullOrEmpty(title))
            if (!Regex.IsMatch(title, "[a-hj-zищъ]", RegexOptions.IgnoreCase)) // латинское i можно
                return title.Replace('I', 'І').Replace('i', 'і').Replace('\'', '’'); // латинское i заменяем на кириллическое

        return null;
    }
}
