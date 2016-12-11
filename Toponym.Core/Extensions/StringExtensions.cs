using System.Text.RegularExpressions;

namespace Toponym.Core.Extensions {
    public static class StringExtensions {

        /// <summary>
        /// Упрощение символов: ё-е, ў-у, «-' и пр.
        /// </summary>
        public static string ToSimple(this string str) {

            str = str.Replace("Ё", "Е").Replace("ё", "е")
                     .Replace("I", "И").Replace("i", "и") // латинская i
                     .Replace("І", "И").Replace("і", "и") // кириллическая і
                     .Replace("Ў", "У").Replace("ў", "у");

            str = new Regex("[\"„“”‘’«»]").Replace(str, "'");
            str = new Regex("[–—−]").Replace(str, "-");
            return str;
        }
    }
}
