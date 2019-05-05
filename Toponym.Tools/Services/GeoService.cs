using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Toponym.Tools.Extensions;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Services
{
    public class GeoService
    {
        public static RelationObject LoadRelation(
            string cacheName, long relationId, string sourcePath = null) =>
            OsmObjectService.LoadRelationObject(
                sourcePath ?? Constants.OsmNewSourcePath, cacheName, relationId);

        public static OsmObjectResponse Load(
            string cacheName, Func<OsmGeo, bool> predicate, string sourcePath = null)
        {
            var response = OsmObjectService.LoadObjects(
                sourcePath ?? Constants.OsmNewSourcePath, cacheName, predicate);

            var allObjects =
                (response.AllNodesDict.Values as IEnumerable<GeoObject>)
                    .Concat(response.AllWaysDict.Values)
                    .Concat(response.AllRelationsDict.Values);

            foreach (var geo in allObjects)
            {
                var titleRu = GeoHelper.TitleRu(geo);
                var titleBe = GeoHelper.TitleBe(geo);
                geo.SetTitleRu(titleRu);
                geo.SetTitleBe(titleBe);
                geo.Title = titleRu;

                if (titleBe != null)
                    geo.Title += " / " + titleBe;
            }

            return response;
        }
    }
}
