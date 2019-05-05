using OsmDataKit;
using System.Collections.Generic;
using Toponym.Core.Models;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Extensions
{
    public static class GeoObjectExtentions
    {
        private const string TitleRuKey = "TitleRu";
        private const string TitleBeKey = "TitleBe";

        public static string TitleRu(this GeoObject osmObject) =>
            osmObject.Data.GetValueOrDefault(TitleRuKey);

        public static string TitleBe(this GeoObject osmObject) =>
            osmObject.Data.GetValueOrDefault(TitleBeKey);

        public static void SetTitleRu(this GeoObject osmObject, string value) =>
            osmObject.Data[TitleRuKey] = value;
        
        public static void SetTitleBe(this GeoObject osmObject, string value) =>
            osmObject.Data[TitleBeKey] = value;

        public static EntryData ToEntryDataAsPoint(this GeoObject geo, EntryType type) =>
            EntryHelper.GetData(
                geo.TitleRu(), geo.TitleBe(), type, geo.AverageCoords);
    }
}
