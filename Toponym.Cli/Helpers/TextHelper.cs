namespace Toponym.Cli;

using OsmDataKit.Logging;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

public static class TextHelper
{
    public static bool IsValidTitleRu(string titleRu)
    {
        var lower = titleRu.ToLower();

        return Regex.IsMatch(titleRu, "^[А-ЯЁ][а-яё]+([ -][А-ЯЁ][а-яё]+)*$") &&
               !lower.Contains("жы") && !lower.Contains("чы") && !lower.Contains("шы") &&
               !lower.Contains("цё") && !lower.Contains("ць") && !lower.Contains("ця");
    }

    public static bool IsValidTitleBe(string? titleBe)
    {
        if (titleBe == null)
            return true;

        return Regex.IsMatch(titleBe, "^[А-ЗЙ-ШЫ-ЯЁІ][а-зй-шы-яёіў’]+([ -][А-ЗЙ-ШЫ-ЯЁІ][а-зй-шы-яёіў’]+)*$") &&
               !Regex.IsMatch(titleBe, "[аеёіоуыэюя]у", RegexOptions.IgnoreCase);
    }

    #region Constants & Maps

    private static readonly Dictionary<string, string> _translitMap =
        new()
        {
            { "а", "a" },
            { "б", "b" },
            { "в", "v" },
            { "г", "g" },
            { "д", "d" },
            { "е", "e" },
            { "ё", "yo" },
            { "ж", "zh" },
            { "з", "z" },
            { "и", "i" },
            { "й", "y" },
            { "к", "k" },
            { "л", "l" },
            { "м", "m" },
            { "н", "n" },
            { "о", "o" },
            { "п", "p" },
            { "р", "r" },
            { "с", "s" },
            { "т", "t" },
            { "у", "u" },
            { "ф", "f" },
            { "х", "h" },
            { "ц", "ts" },
            { "ч", "ch" },
            { "ш", "sh" },
            { "щ", "sch" },
            { "ъ", "" },
            { "ы", "i" },
            { "ь", "" },
            { "э", "e" },
            { "ю", "yu" },
            { "я", "ya" },
            { "і", "i" }, // cyrillic "i"
            { "'", "’" },
            { "`", "’" },
            { "ʼ", "’" }, // 700 \u02bc => 8217 \u2009
            //todo
            { "ї", "yi" },
            { "ө", "s" },
            { "ү", "u" },
            { "ҷ", "ch" },
            { "ӣ", "i" },
            { "ӯ", "u" },
            { "ҳ", "h" },
            { "қ", "k" },
            { "ғ", "g" },
            { "ң", "n" },
            { "є", "e" },
            { "ј", "y" }, // cyrillic "j"
            { "љ", "l" },
            { "ћ", "h" },
            { "џ", "ts" },
            { "ќ", "k" },
            { "њ", "n" },
            { "ђ", "h" },
            { "ѓ", "g" },
            { "ґ", "g" },
            { "ѝ", "i" },
            { "ѐ", "e" }
        };

    private static readonly string _cyrillicToLatinPattern =
        string.Join(string.Empty, _translitMap.Keys.Select(i => i.ToUpper()).Concat(_translitMap.Keys));

    #endregion

    public static string FixApostrophe(string text) => Regex.Replace(text, "[`'‘ʻʼ]", "’");

    public static string CyrillicToLatin(string text)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(text));

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text));

        var sb = new StringBuilder(text);
        sb.Replace("ее", "eye");
        sb.Replace("ье", "ye");
        sb.Replace("ьи", "yi");
        sb.Replace("ъе", "ye");
        sb.Replace("ъи", "yi");
        var sbString = sb.ToString();

        var result = Regex.Replace(
            sbString, $"[{_cyrillicToLatinPattern}]", i =>
            {
                var isUpper = i.Value == i.Value.ToUpper();
                var translited = _translitMap[i.Value.ToLower()];

                return isUpper && translited.Length > 0
                    ? translited[0].ToString().ToUpper() + translited[1..]
                    : translited;
            });

        if (!Regex.IsMatch(result.ToLower(), @"^[a-z ’-]+$"))
            Logger.Warning($"Non-translited cyrillic title: {result}");

        return result;
    }
}
