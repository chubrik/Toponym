using Kit;
using OsmDataKit;
using OsmSharp;

namespace Toponym.Tools
{
    public static class GeoService
    {
        public static RelationObject LoadRelation(string cacheName, long relationId, string sourcePath)
        {
            var request = new GeoRequest { RelationIds = new[] { relationId } };
            var response = OsmService.LoadCompleteObjects(sourcePath, cacheName, request);
            var relation = response.RootRelations.Single();
            SetTitles(relation);
            return relation;
        }

        public static CompleteGeoObjects Load(string cacheName, Func<OsmGeo, bool> predicate, string sourcePath)
        {
            var response = OsmService.LoadCompleteObjects(sourcePath, cacheName, predicate);

            return LogService.Log("Set titles", () =>
            {
                foreach (var geo in response.AllObjects())
                    SetTitles(geo);

                foreach (var relation in response.RootRelations.Where(i => i.TitleBe() == null))
                {
                    var label = NotNull(relation.Members).FirstOrDefault(i => i.Role == "label");

                    if (label != null)
                    {
                        var labelTitleBe = label.Geo.TitleBe();

                        if (labelTitleBe != null)
                            relation.SetTitleBe(labelTitleBe);
                    }
                }

                GeoObject.StringFormatter = geo =>
                {
                    var titleRu = geo.TitleRu() ?? "<null>";
                    var titleBe = geo.TitleBe();
                    return titleBe != null ? titleRu + " / " + titleBe : titleRu;
                };

                return response;
            });
        }

        private static void SetTitles(GeoObject geo)
        {
            var titleRu = GeoHelper.TitleRu(geo);
            var titleBe = GeoHelper.TitleBe(geo);

            if (titleRu != null)
                geo.SetTitleRu(titleRu);

            if (titleBe != null)
                geo.SetTitleBe(titleBe);
        }
    }
}
