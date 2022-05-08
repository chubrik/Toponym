using Kit;
using OsmDataKit;
using OsmSharp;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class PopulatedService
    {
        public static List<EntryData> Build()
        {
            return LogService.InfoSuccess("Build populated", () =>
            {
                var response = GeoService.Load(
                    "populated",
                    i => ((i.Tags.ContainsKey("place") && !i.Tags.Contains("place", "locality")) ||
                          i.Tags.ContainsKey("old_place") ||
                          i.Tags.ContainsKey("abandoned:place") ||
                          i.Tags.Contains("landuse", "residential")) &&
                         GeoHelper.TitleRu(i) != null,
                         Constants.Osm2019SourcePath);

                LogService.Info("Filter & fix");

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

                var data = final.Select(GetEntryData).ToSortedList();
                FixMinskCenter(data);
                FileHelper.WriteData(Constants.PopulatedDataPath, data);
                return data;
            });
        }

        private static bool FilterByType(GeoObject geo)
        {
            if (!NotNull(geo.Tags).ContainsKey("place"))
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
            var titleRu = NotNull(geo.TitleRu());

            if (titleRu.Contains("Блок Пост") ||
                titleRu.Contains("Военный") ||
                titleRu.Contains("Годовщина") ||
                titleRu.Contains("льнозавод") ||
                titleRu.Contains("Поместье") ||
                titleRu.Contains("Станция") ||
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
                var label = NotNull(relation.Members).FirstOrDefault(i => i.Role == "label");

                if (label != null)
                {
                    //Debug.Assert(label.Geo.TitleRu() == geo.TitleRu());

                    if (relation.TitleBe() == null)
                    {
                        var labelTitleBe = label.Geo.TitleBe();

                        if (labelTitleBe != null)
                            geo.SetTitleBe(labelTitleBe);
                    }
                }
            }

            var titleRu = NotNull(geo.TitleRu());
            var titleBe = geo.TitleBe();

            if (titleRu == "Заборье (ферма)")
                geo.SetTitleRu("Заборье");

            if (titleRu == "Залесье (бывшее имение)")
                geo.SetTitleRu("Залесье");

            if (titleRu == "Клины нежил.")
                geo.SetTitleRu("Клины");

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

            return geo;
        }

        private static bool IsInside(GeoObject area, NodeObject node)
        {
            var areaNodes = NotNull(
                area is WayObject way
                    ? way.Nodes
                    : area is RelationObject relation
                        ? relation.AllChildNodes()
                        : throw new InvalidOperationException());

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

            var entryType = placeType switch
            {
                "city" => EntryType.City,
                "hamlet" => EntryType.Hamlet,
                "isolated_dwelling" => EntryType.Dwelling,
                "town" => EntryType.Town,
                "village" => EntryType.Village,
                _ => throw new ArgumentOutOfRangeException(nameof(geo)),
            };

            var geoType = geo.Type;

            return geoType switch
            {
                OsmGeoType.Node => EntryHelper.GetData(
                    NotNull(geo.TitleRu()), geo.TitleBe(), entryType, ((NodeObject)geo).Location),
                OsmGeoType.Way => ((WayObject)geo).ToEntryDataAsPoint(entryType),
                OsmGeoType.Relation => ((RelationObject)geo).ToEntryDataAsPoint(entryType),
                _ => throw new ArgumentOutOfRangeException(nameof(geo)),
            };
        }

        private static string GetPlaceType(GeoObject geo)
        {
            var geoTags = NotNull(geo.Tags);

            if (geoTags.TryGetValue("old_place", out var result) ||
                geoTags.TryGetValue("abandoned:place", out result))
                return result;

            return geoTags["place"];
        }

        private static void FixMinskCenter(List<EntryData> data)
        {
            var minsk = data.Single(i => i.TitleRu == "Минск");
            var location = new Location(53.90234, 27.56188);
            var fakeEntry = EntryHelper.GetData(minsk.TitleRu, minsk.TitleBe, minsk.Type, location);
            minsk.Relocate(location: fakeEntry.Location, screenPoints: fakeEntry.ScreenPoints);
        }
    }
}
