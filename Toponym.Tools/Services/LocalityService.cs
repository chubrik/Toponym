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
            LogService.LogInfo("Build localities");

            var response = GeoService.Load(
                "localities",
                i => i.Tags.Contains("place", "locality") &&
                GeoHelper.TitleRu(i) != null);

            LogService.Log("Filter & fix");

            var filtered = response.RootObjects().Where(Filter).Select(Fix).ToList();
            var data = filtered.Select(GetEntryData).OrderBy(i => i.TitleRu).ToList();
            JsonFileClient.Write(Constants.LocalitiesDataPath, data);
            LogService.LogInfo("Build populated complete");
            return data;
        }

        private static bool Filter(GeoObject geo)
        {
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
                normRu == "рыбалка" ||
                normRu == "рэдки алёс" ||
                normRu == "ст дорожник" ||
                normRu.Contains("торф.") ||
                normRu == "тросцянец" ||
                normRu == "фелиццяново")
                return false;

            return true;
        }

        private const string TitleNumPattern = @"^.+(?=[ -]\d(-е)?$)";

        private static GeoObject Fix(GeoObject geo)
        {
            var titleRu = geo.TitleRu();

            var match1 = Regex.Match(titleRu, @"^ур(очище|\.)(.+)$", RegexOptions.IgnoreCase);

            if (match1.Success)
                titleRu = match1.Groups[2].Value.Trim();

            var matchRu = Regex.Match(titleRu, TitleNumPattern);

            if (matchRu.Success)
                titleRu = matchRu.Value;

            if (titleRu == "Клины нежил.")
                titleRu = "Клины";

            if (titleRu == "Пухичин (нежил.)")
                titleRu = "Пухичин";

            var match2 = Regex.Match(titleRu, @"^(\w+) ([а-яё])(\w+)$");

            if (match2.Success)
            {
                var groups = match2.Groups;
                titleRu = groups[1] + " " + groups[2].ToString().ToUpper() + groups[3];
            }

            geo.SetTitleRu(titleRu);

            // TitleBe

            var titleBe = geo.TitleBe();

            if (titleBe != null)
            {
                var match3 = Regex.Match(titleBe, @"^(у|ў)рочыш.а (.+)$", RegexOptions.IgnoreCase);

                if (match3.Success)
                    titleBe = match3.Groups[2].Value;

                var matchBe = Regex.Match(titleBe, TitleNumPattern);

                if (matchBe.Success)
                    titleBe = matchBe.Value;

                if (titleBe == "Бярэ́зіна")
                    titleBe = "Бярэзіна";

                if (titleBe == "руч. Муравец")
                    titleBe = "Муравец";

                geo.SetTitleBe(titleBe);
            }

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
