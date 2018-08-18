using OsmDataKit.Models;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Toponym.Tools.Helpers
{
    public static class OsmHelper
    {
        private static readonly List<string> _tagRuNames = new List<string> { "name", "int_name", "alt_name" };

        public static string TitleRu(OsmGeo geo)
        {
            Debug.Assert(geo != null);

            if (geo == null)
                throw new ArgumentNullException(nameof(geo));

            return TitleRuBase(geo.Tags.ToDictionary(i => i.Key, i => i.Value));
        }

        public static string TitleRu(OsmObject geo)
        {
            Debug.Assert(geo != null);

            if (geo == null)
                throw new ArgumentNullException(nameof(geo));

            return TitleRuBase(geo.Tags);
        }

        private static string TitleRuBase(IReadOnlyDictionary<string, string> tags)
        {
            if (tags.Count == 0)
                return null;

            if (tags.TryGetValue("name:ru", out string title) && !string.IsNullOrEmpty(title))
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

        public static string TitleBe(OsmObject geo)
        {
            Debug.Assert(geo != null);

            if (geo == null)
                throw new ArgumentNullException(nameof(geo));

            if (geo.Tags.TryGetValue("name:be", out string title) && !string.IsNullOrEmpty(title))
                if (!Regex.IsMatch(title, "[a-hj-zищъ]", RegexOptions.IgnoreCase)) // латинское i можно
                    return title.Replace('I', 'І').Replace('i', 'і').Replace('\'', '’'); // латинское i заменяем на кириллическое

            return null;
        }
    }
}
