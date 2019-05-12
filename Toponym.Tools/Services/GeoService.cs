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
            var response = OsmObjectService.LoadObjects(sourcePath ?? Constants.OsmNewSourcePath, cacheName, request);
            return response.RootRelations.Values.Single();
        }

        public static OsmObjectResponse Load(string cacheName, Func<OsmGeo, bool> predicate, string sourcePath = null)
        {
            var response = OsmObjectService.LoadObjects(
                sourcePath ?? Constants.OsmNewSourcePath, cacheName, predicate);

            foreach (var geo in response.AllObjects())
            {
                var titleRu = GeoHelper.TitleRu(geo);
                var titleBe = GeoHelper.TitleBe(geo);
                geo.SetTitleRu(titleRu);
                geo.SetTitleBe(titleBe);
            }

            GeoObject.TitleFormatter = geo =>
            {
                var titleRu = geo.TitleRu();
                var titleBe = geo.TitleBe();
                return titleBe != null ? titleRu + " / " + titleBe : titleRu;
            };

            return response;
        }
    }
}
