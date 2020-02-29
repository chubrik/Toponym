using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Toponym.Core.Models;
using Toponym.Tools.Extensions;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Services
{
    public class LocalityService
    {
        public static List<EntryData> Build()
        {
            LogService.BeginInfo("Build localities");

            var response = GeoService.Load(
                "localities",
                i => i.Tags.Contains("place", "locality") &&
                GeoHelper.TitleRu(i) != null);

            var geos = response.RootObjects().ToList();
            var rejected = geos.Where(i => !Filter(i)).ToList();
            HtmlHelper.Write("localities-rejected", rejected);

            var proceeded = geos.Where(Filter).Select(Proceed).ToList();
            //HtmlHelper.Write("localities", proceeded);

            var data = proceeded.Select(GetEntryData).ToSortedList();
            JsonFileClient.Write(Constants.LocalitiesDataPath, data);
            LogService.EndSuccess("Build populated completed");
            return data;
        }

        private static bool Filter(GeoObject geo)
        {
            if (geo.Type == OsmGeoType.Node)
                switch (geo.Id)
                {
                    case 5737788153: // Фелиццяново
                        return false;
                    case 5737801570: // Остров Лещ / востраў Лешч //todo water
                        return false;
                    case 5737802406: // Тросцянец
                        return false;
                    case 7198437795: // ур. Єфремове - за границей
                        return false;
                    case 7230336501: // ур. Ду́рний - за границей
                        return false;
                }

            if (geo.Type == OsmGeoType.Relation)
                switch (geo.Id)
                {
                    case 7670569: // Рэдки Алёс
                        return false;
                }

            var normRu = geo.TitleRu().ToLower();

            if (Regex.IsMatch(normRu, @"\d"))
                return false;

            if (normRu.Contains("аэродром") ||
                normRu.Contains("жкх") ||
                normRu.Contains("спутник") ||
                normRu.Contains("лагель") ||
                normRu.Contains("лагерь") ||
                normRu.Contains("лесничество") ||
                normRu.Contains("сооружения") ||
                normRu.Contains("санаторий") ||
                normRu.Contains("торф."))
                return false;

            return true;
        }

        private static GeoObject Proceed(GeoObject geo)
        {
            var titleRu = Regex.Replace(geo.TitleRu(),
                @"^ур(очище |\.)", "", RegexOptions.IgnoreCase).Trim();

            var matchRu = Regex.Match(titleRu, @"^(\w+) ([а-яё])(\w+)$");

            if (matchRu.Success)
            {
                var groups = matchRu.Groups;
                titleRu = groups[1] + " " + groups[2].ToString().ToUpper() + groups[3];
            }

            var titleBe = geo.TitleBe();

            if (titleBe != null)
            {
                titleBe = Regex.Replace(titleBe,
                    @"^[уў]рочышча ", "", RegexOptions.IgnoreCase).Trim();

                var matchBe = Regex.Match(titleBe, @"^(\w+) ([а-яёіў])(\w+)$");

                if (matchBe.Success)
                {
                    var groups = matchBe.Groups;
                    titleBe = groups[1] + " " + groups[2].ToString().ToUpper() + groups[3];
                }
            }

            var id = geo.Id;

            if (geo.Type == OsmGeoType.Node)
            {
                if (id == 243043205) // Березина / Бярэ́зіна
                    titleBe = "Бярэзіна";

                if (id == 243046675) // Клины нежил.
                    titleRu = "Клины";

                if (id == 243047300) // Муравец / руч. Муравец
                    titleBe = "Муравец";

                if (id == 3280099301) // Красная Горка (операция Багратион)
                    titleRu = "Красная Горка";

                if (id == 5209063019) // гора Яшукова
                    titleRu = "Гора Яшукова";

                if (id == 6408356104) // Хоцяновичи
                    titleRu = "Хотяновичи";
            }

            if (!TextHelper.IsValidTitleRu(titleRu) ||
                !TextHelper.IsValidTitleBe(titleBe))
                throw new InvalidOperationException();

            geo.SetTitleRu(titleRu);
            geo.SetTitleBe(titleBe);
            return geo;
        }

        private static EntryData GetEntryData(GeoObject geo)
        {
            var geoType = geo.Type;

            switch (geoType)
            {
                case OsmGeoType.Node:
                    return EntryHelper.GetData(geo.TitleRu(), geo.TitleBe(), EntryType.Locality, (NodeObject)geo);

                case OsmGeoType.Way:
                    return ((WayObject)geo).ToEntryDataAsPoint(EntryType.Locality);

                case OsmGeoType.Relation:
                    return ((RelationObject)geo).ToEntryDataAsPoint(EntryType.Locality);

                default:
                    throw new ArgumentOutOfRangeException(nameof(geoType));
            }
        }
    }
}
