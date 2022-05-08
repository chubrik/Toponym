using Kit;
using OsmDataKit;
using OsmSharp;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class LocalityService
    {
        public static List<EntryData> Build()
        {
            return LogService.InfoSuccess("Build localities", () =>
            {
                var response = GeoService.Load(
                    "localities",
                    i => i.Tags.Contains("place", "locality") &&
                    GeoHelper.TitleRu(i) != null,
                    Constants.Osm2019SourcePath);

                LogService.Info("Filter & fix");

                var filtered = response.RootObjects().Where(Filter).Select(Fix).ToList();
                var data = filtered.Select(GetEntryData).ToSortedList();
                FileHelper.WriteData(Constants.LocalitiesDataPath, data);
                return data;
            });
        }

        private static bool Filter(GeoObject geo)
        {
            var normRu = NotNull(geo.TitleRu()).ToLower();

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
                normRu.Contains("торф.") ||
                normRu == "кирпичный завод" ||
                normRu == "рыбалка" ||
                normRu == "рэдки алёс" ||
                normRu == "ст дорожник" ||
                normRu == "тросцянец" ||
                normRu == "фелиццяново")
                return false;

            return true;
        }

        private const string TitleNumPattern = @"^.+(?=[ -]\d(-е)?$)";

        private static GeoObject Fix(GeoObject geo)
        {
            var titleRu = NotNull(geo.TitleRu());

            var match1 = Regex.Match(titleRu, @"^ур(очище|\.)(.+)$", RegexOptions.IgnoreCase);

            if (match1.Success)
                titleRu = match1.Groups[2].Value.Trim();

            var matchRu = Regex.Match(titleRu, TitleNumPattern);

            if (matchRu.Success)
                titleRu = matchRu.Value;

            if (titleRu == "Клины нежил.")
                titleRu = "Клины";

            if (titleRu == "Красная Горка (операция Багратион)")
                titleRu = "Красная Горка";

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

                if (titleBe == "Высокая горка")
                    titleBe = "Высокая Горка";

                if (titleBe == "Высокі бераг")
                    titleBe = "Высокі Бераг";

                if (titleBe == "руч. Муравец")
                    titleBe = "Муравец";

                if (titleBe == "Хваёвы сад")
                    titleBe = "Хваёвы Сад";

                geo.SetTitleBe(titleBe);
            }

            return geo;
        }

        private static EntryData GetEntryData(GeoObject geo)
        {
            var geoType = geo.Type;

            return geoType switch
            {
                OsmGeoType.Node => EntryHelper.GetData(
                    NotNull(geo.TitleRu()), geo.TitleBe(), EntryType.Locality, ((NodeObject)geo).Location),
                OsmGeoType.Way => ((WayObject)geo).ToEntryDataAsPoint(EntryType.Locality),
                OsmGeoType.Relation => ((RelationObject)geo).ToEntryDataAsPoint(EntryType.Locality),
                _ => throw new ArgumentOutOfRangeException(nameof(geo)),
            };
        }
    }
}
