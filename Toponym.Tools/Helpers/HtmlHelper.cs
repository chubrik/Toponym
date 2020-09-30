using Kit;
using OsmDataKit;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            FileClient.WriteText($"{title}.html", html);
        }

        public static void Write(string title, IEnumerable<EntryData> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table>");
            var number = 1;

            foreach (var entry in data.OrderBy(i => i.TitleRu))
            {
                var lat = entry.Location[0];
                var lng = entry.Location[1];
                sb.Append($"<tr>");
                sb.Append($"<td>{number++}.</td>");
                sb.Append($"<td>{entry.TitleRu}</td>");
                sb.Append($"<td>{entry.TitleBe}</td>");
                sb.Append($"<td>{entry.Type}</td>");
                sb.Append($"<td>{lat}, {lng}</td>");
                sb.Append($"<td>");
                sb.Append($"<a target=\"_blank\" href=\"https://www.openstreetmap.org/?mlat={lat}&mlon={lng}&zoom=14\">O</a> ");
                sb.Append($"<a target=\"_blank\" href=\"https://www.google.ru/maps/place//@{lat},{lng},5000m/data=!3m1!1e3!4m2!3m1!1s0x0:0x0?hl=ru\">G</a> ");
                sb.Append($"<a target=\"_blank\" href=\"https://yandex.ru/maps?ll={lng},{lat}&pt={lng},{lat}&z=14&l=sat%2Cskl\">Я</a> ");
                sb.Append($"<a target=\"_blank\" href=\"http://m.loadmap.net/ru?qq={lat}%20{lng}&z=13&s=100000&c=41&g=1\">L</a>");
                sb.Append($"</td>");
                sb.AppendLine($"</tr>");
            }

            sb.AppendLine("</table>");
            var html = sb.ToString();
            FileClient.WriteText($"{title}.html", html);
        }

        public static void Write(string title, IEnumerable<(EntryData, EntryData)> data)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table>");
            var number = 1;

            foreach (var pair in data.OrderBy(i => i.Item1.TitleRu))
            {
                var item1 = pair.Item1;
                var item2 = pair.Item2;
                var lat = item2.Location[0];
                var lng = item2.Location[1];
                sb.Append($"<tr>");
                sb.Append($"<td>{number++}.</td>");
                sb.Append($"<td>{item1.TitleRu}</td>");
                sb.Append($"<td>{item2.TitleRu}</td>");
                sb.Append($"<td>{item1.TitleBe}</td>");
                sb.Append($"<td>{item2.TitleBe}</td>");
                sb.Append($"<td>{item1.Type}</td>");
                sb.Append($"<td>{item2.Type}</td>");
                sb.Append($"<td>{lat}, {lng}</td>");
                sb.Append($"<td>");
                sb.Append($"<a target=\"_blank\" href=\"https://www.openstreetmap.org/?mlat={lat}&mlon={lng}&zoom=14\">O</a> ");
                sb.Append($"<a target=\"_blank\" href=\"https://www.google.ru/maps/place//@{lat},{lng},5000m/data=!3m1!1e3!4m2!3m1!1s0x0:0x0?hl=ru\">G</a> ");
                sb.Append($"<a target=\"_blank\" href=\"https://yandex.ru/maps?ll={lng},{lat}&pt={lng},{lat}&z=14&l=sat%2Cskl\">Я</a> ");
                sb.Append($"<a target=\"_blank\" href=\"http://m.loadmap.net/ru?qq={lat}%20{lng}&z=13&s=100000&c=41&g=1\">L</a>");
                sb.Append($"</td>");
                sb.AppendLine($"</tr>");
            }

            sb.AppendLine("</table>");
            var html = sb.ToString();
            FileClient.WriteText($"{title}.html", html);
        }
    }
}
