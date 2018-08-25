using Kit;
using OsmDataKit.Models;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
                OsmHelper.TitleRu(i) != null);

            var allGeos = (response.Nodes as IEnumerable<OsmObject>)
                .Concat(response.Ways)
                .Concat(response.Relations);

            LogService.Log("Filter & fix");

            var filtered = allGeos.Where(Filter).Select(Fix).ToList();
            var data = filtered.Select(GetEntryData).ToList();
            JsonFileClient.Write(Constants.LocalitiesDataPath, data);
            LogService.LogInfo("Build populated complete");
            return data;
        }

        private static bool Filter(OsmObject geo)
        {
            var titleRu = geo.TitleRu();

            return true;
        }

        private static OsmObject Fix(OsmObject geo)
        {
            return geo;
        }

        private static EntryData GetEntryData(OsmObject geo)
        {
            var geoType = geo.Type;

            switch (geoType)
            {
                case OsmGeoType.Node:
                    return EntryHelper.GetData(geo.TitleRu(), geo.TitleBe(), EntryType.Locality, (OsmNode)geo);

                case OsmGeoType.Way:
                    return ((OsmWay)geo).ToEntryDataPoint(EntryType.Locality);

                case OsmGeoType.Relation:
                    return ((OsmRelation)geo).ToEntryDataPoint(EntryType.Locality);

                default:
                    throw new ArgumentOutOfRangeException(nameof(geoType));
            }
        }
    }
}
