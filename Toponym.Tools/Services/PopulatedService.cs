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
    public class PopulatedService
    {
        public static List<EntryData> Build()
        {
            LogService.LogInfo("Build populated");

            var response = GeoService.Load(
                "populated",
                i => ((i.Tags.ContainsKey("place") && !i.Tags.Contains("place", "locality")) ||
                      i.Tags.ContainsKey("old_place") ||
                      i.Tags.ContainsKey("abandoned:place") ||
                      i.Tags.Contains("landuse", "residential")) &&
                     GeoHelper.TitleRu(i) != null);

            LogService.Log("Filter & fix");

            var filteredByType = response.RootObjects().Where(FilterByType).OrderBy(i => i.TitleRu()).ToList();
            var badNames = filteredByType.Where(i => !FilterByName(i)).ToList();

            var filtered = filteredByType.Where(FilterByName).Select(Fix).ToList();
            var nodes = filtered.Where(i => i.Type == OsmGeoType.Node).Select(i => (NodeObject)i).ToList();
            var final = filtered.Where(i => i.Type != OsmGeoType.Node).ToList();
            var areasLookup = final.ToLookup(i => i.TitleRu());

            foreach (var node in nodes)
            {
                var title = node.TitleRu();
                var areas = areasLookup[title];

                if (areas.All(i => !IsInside(i, node)))
                    final.Add(node);
            }

            var data = final.Select(GetEntryData).ToList();
            FixMinskCenter(data);
            JsonFileClient.Write(Constants.PopulatedDataPath, data);
            LogService.LogInfo("Build populated complete");
            return data;
        }

        private static bool FilterByType(GeoObject geo)
        {
            if (!geo.Tags.ContainsKey("place"))
                return false;

            var placeType = GetPlaceType(geo);

            if (placeType != "city" &&
                placeType != "hamlet" &&
                placeType != "isolated_dwelling" &&
                placeType != "town" &&
                placeType != "village")
                return false;

            return true;
        }

        private static bool FilterByName(GeoObject geo)
        {
            var titleRu = geo.TitleRu();

            if (titleRu.Contains("Блок Пост") ||
                titleRu.Contains("Военный") ||
                titleRu.Contains("Годовщина") ||
                titleRu.Contains("льнозавод") ||
                titleRu.Contains("Университетский") ||
                titleRu.Contains("урочище") || //todo locality
                titleRu == "163 км" ||
                titleRu == "Wołyńce" ||
                titleRu == "Посёлок N4" ||
                titleRu == "Посёлок №9" ||
                titleRu == "РТС" ||
                titleRu == "с.т Тюльпан")
                return false;

            var titleBe = geo.TitleBe();

            if (titleBe != null &&
                titleBe.Contains("урочышча")) //todo locality
                return false;

            return true;
        }

        private const string TitleNumPattern = @"^.+(?=[ -]\d(-е)?$)";

        private static GeoObject Fix(GeoObject geo)
        {
            if (geo is RelationObject relation)
            {
                var label = relation.Members.FirstOrDefault(i => i.Role == "label");

                if (label != null)
                {
                    //Debug.Assert(label.Geo.TitleRu() == geo.TitleRu());

                    if (relation.TitleBe() == null)
                        geo.SetTitleBe(label.Geo.TitleBe());
                }
            }

            var titleRu = geo.TitleRu();
            var titleBe = geo.TitleBe();

            if (titleRu == "Заборье (ферма)")
                geo.SetTitleRu("Заборье");

            if (titleRu == "Клины нежил.")
                geo.SetTitleRu("Клины");

            if (titleRu == "Шиловичи (спиртзавод)")
            {
                geo.SetTitleRu("Шиловичи");
                geo.SetTitleBe("Шылавічы");
            }

            if (titleBe == "Аульс")
                geo.SetTitleBe("Авульс");

            if (titleBe == "Бліунг")
                geo.SetTitleBe("Бліўнг");

            if (titleBe == "Бярозавая Рошча (Коньскі Бор)")
                geo.SetTitleBe("Бярозавая Рошча");

            if (titleBe == "Бярэ́зіна")
                geo.SetTitleBe("Бярэзіна");

            if (titleBe == "Калауравічы")
                geo.SetTitleBe("Калавуравічы");

            if (titleBe == "руч. Муравец")
                geo.SetTitleBe("Муравец");

            if (titleBe == "Рэут")
                geo.SetTitleBe("Рэвут");

            var matchRu = Regex.Match(titleRu, TitleNumPattern);

            if (matchRu.Success)
                geo.SetTitleRu(matchRu.Value);

            if (titleBe != null)
            {
                var matchBe = Regex.Match(titleBe, TitleNumPattern);

                if (matchBe.Success)
                    geo.SetTitleBe(matchBe.Value);
            }

            if (!TextHelper.IsValidTitleRu(geo.TitleRu()) ||
                !TextHelper.IsValidTitleBe(geo.TitleBe()))
            {
            }

            return geo;
        }

        private static bool IsInside(GeoObject area, NodeObject node)
        {
            var areaNodes =
                area is WayObject way
                    ? way.Nodes
                    : area is RelationObject relation
                        ? relation.AllNodes()
                        : throw new InvalidOperationException();

            var top = areaNodes.Max(i => i.Latitude);
            var bottom = areaNodes.Min(i => i.Latitude);
            var left = areaNodes.Min(i => i.Longitude);
            var right = areaNodes.Max(i => i.Longitude);

            return
                node.Latitude < top &&
                node.Latitude > bottom &&
                node.Longitude > left &&
                node.Longitude < right;
        }

        private static EntryData GetEntryData(GeoObject geo)
        {
            var placeType = GetPlaceType(geo);
            EntryType entryType;

            switch (placeType)
            {
                case "city":
                    entryType = EntryType.City;
                    break;

                case "hamlet":
                    entryType = EntryType.Hamlet;
                    break;

                case "isolated_dwelling":
                    entryType = EntryType.Dwelling;
                    break;

                case "town":
                    entryType = EntryType.Town;
                    break;

                case "village":
                    entryType = EntryType.Village;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(placeType));
            }

            var geoType = geo.Type;

            switch (geoType)
            {
                case OsmGeoType.Node:
                    return EntryHelper.GetData(geo.TitleRu(), geo.TitleBe(), entryType, (NodeObject)geo);

                case OsmGeoType.Way:
                    return ((WayObject)geo).ToEntryDataAsPoint(entryType);

                case OsmGeoType.Relation:
                    return ((RelationObject)geo).ToEntryDataAsPoint(entryType);

                default:
                    throw new ArgumentOutOfRangeException(nameof(geoType));
            }
        }

        private static string GetPlaceType(GeoObject geo)
        {
            if (geo.Tags.TryGetValue("old_place", out var result) ||
                geo.Tags.TryGetValue("abandoned:place", out result))
                return result;

            return geo.Tags["place"];
        }

        private static void FixMinskCenter(List<EntryData> data)
        {
            var minsk = data.Single(i => i.TitleRu == "Минск");
            var coords = new GeoCoords(53.90234, 27.56188);
            var fakeEntry = EntryHelper.GetData(minsk.TitleRu, minsk.TitleBe, minsk.Type, coords);
            minsk.Coords = fakeEntry.Coords;
            minsk.Screen = fakeEntry.Screen;
        }
    }
}
