using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Linq;

namespace Toponym.Tools
{
    public static class GeoService
    {
        public static RelationObject LoadRelation(string cacheName, long relationId, string sourcePath = null)
        {
            var request = new GeoRequest { RelationIds = new[] { relationId } };
            var response = OsmService.LoadCompleteObjects(sourcePath ?? Constants.OsmNewSourcePath, cacheName, request);
            return response.RootRelations.Single();
        }

        public static CompleteGeoObjects Load(string cacheName, Func<OsmGeo, bool> predicate, string sourcePath = null)
        {
            var response = OsmService.LoadCompleteObjects(
                sourcePath ?? Constants.OsmNewSourcePath, cacheName, predicate);

            LogService.Begin("Set titles");

            foreach (var geo in response.AllObjects())
            {
                var titleRu = GeoHelper.TitleRu(geo);
                var titleBe = GeoHelper.TitleBe(geo);
                geo.SetTitleRu(titleRu);
                geo.SetTitleBe(titleBe);
            }

            foreach (var relation in response.RootRelations.Where(i => i.TitleBe() == null))
            {
                var label = relation.Members.FirstOrDefault(i => i.Role == "label");

                if (label != null)
                {
                    var labelTitleBe = label.Geo.TitleBe();

                    if (labelTitleBe != null)
                        relation.SetTitleBe(labelTitleBe);
                }
            }

            GeoObject.StringFormatter = geo =>
            {
                var titleRu = geo.TitleRu();
                var titleBe = geo.TitleBe();
                return titleBe != null ? titleRu + " / " + titleBe : titleRu;
            };

            LogService.End("Set titles");
            return response;
        }
    }
}
