using Kit;
using OsmDataKit;
using System.Collections.Generic;
using System.Linq;

namespace Toponym.Tools
{
    public static class HtmlHelper
    {
        public static void Write(string title, IEnumerable<GeoObject> geos)
        {
            var html = "<style>a{text-decoration:none}</style><div style=\"column-count: 10\">";

            foreach (var geo in geos.OrderBy(i => i.TitleRu()))
                html += "<a href=\"https://" +
                        $"www.openstreetmap.org/{geo.Type.ToString().ToLower()}/{geo.Id}\" " +
                        $"target=\"_blank\">{geo.TitleRu()}</a><br>\n";

            html += "<div>";
            FileClient.Write($"{title}.html", html);
        }
    }
}
