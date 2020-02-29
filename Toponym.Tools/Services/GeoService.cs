using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Linq;
using Toponym.Tools.Extensions;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Services
{
    public class GeoService
    {
        public static RelationObject LoadRelation(string cacheName, long relationId, string sourcePath = null)
        {
            var request = new OsmRequest { RelationIds = new[] { relationId } };
            var response = OsmService.LoadObjects(sourcePath ?? Constants.OsmNewSourcePath, cacheName, request);
            return response.RootRelations.Values.Single();
        }

        public static OsmObjectResponse Load(string cacheName, Func<OsmGeo, bool> predicate, string sourcePath = null)
        {
            var response = OsmService.LoadObjects(
                sourcePath ?? Constants.OsmNewSourcePath, cacheName, predicate);

            LogService.Begin("Set titles");

            foreach (var geo in response.DeepObjects())
            {
                var titleRu = GeoHelper.TitleRu(geo);
                var titleBe = GeoHelper.TitleBe(geo);
                geo.SetTitleRu(titleRu);
                geo.SetTitleBe(titleBe);
            }

            foreach (var relation in response.RootRelations.Values.Where(i => i.TitleBe() == null))
            {
                var label = relation.Members.FirstOrDefault(i => i.Role == "label");

                if (label != null)
                {
                    var labelTitleBe = label.Geo.TitleBe();

                    if (labelTitleBe != null)
                        relation.SetTitleBe(labelTitleBe);
                }
            }

            GeoObject.TitleFormatter = geo =>
            {
                var titleRu = geo.TitleRu();
                var titleBe = geo.TitleBe();
                return titleBe != null ? titleRu + " / " + titleBe : titleRu;
            };

            LogService.End("Set titles completed");
            return response;
        }
    }
}
