using System;
using System.Linq;
using System.Text.RegularExpressions;
using Toponym.Site.Models;

namespace Toponym.Site.Helpers
{
    public static class LangHelper
    {
        public static Language GetByQueryParam(string queryParam)
        {
            switch (queryParam)
            {
                case "ru":
                    return Language.Russian;

                case "be":
                    return Language.Belarusian;

                case "en":
                    return Language.English;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static string Text(Language language, string russian, string belarusian, string english)
        {
            switch (language)
            {
                case Language.Russian:
                    return russian;

                case Language.Belarusian:
                    return belarusian;

                case Language.English:
                    return english;

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }
        }

        public static string RusCase(int number, string[] cases, bool includeNumber = true)
        {
            if (cases == null)
                throw new ArgumentNullException(nameof(cases));

            if (cases.Length < 2 || cases.Length > 3 || cases.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException(nameof(cases));

            var num = number.ToString();
            var result = includeNumber ? num + " " : "";

            if (num.Length > 1 && num[num.Length - 2] == '1')
                return result + (cases[2] ?? cases[1]);

            if (num.Last() == '1')
                return result + cases[0];

            if (Regex.IsMatch(num.Last().ToString(), "[234]"))
                return result + cases[1];

            return result + (cases[2] ?? cases[1]);
        }
    }
}
