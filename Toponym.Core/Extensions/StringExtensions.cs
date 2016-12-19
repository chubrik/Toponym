using System.Text.RegularExpressions;

namespace Toponym.Core.Extensions {
    public static class StringExtensions {

        public static string ToBase(this string str) {

            str = str.Replace("Ё", "Е").Replace("ё", "е")
                     .Replace("I", "И").Replace("i", "и") // Latin "i"
                     .Replace("І", "И").Replace("і", "и") // Cyrillic "і"
                     .Replace("Ў", "У").Replace("ў", "у");

            str = new Regex("[\"„“”‘’`«»]").Replace(str, "'");
            str = new Regex("[–—−]").Replace(str, "-");
            return str;
        }
    }
}
