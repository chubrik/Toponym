using OsmDataKit;
using System.Collections.Generic;
using Toponym.Core.Models;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Extensions
{
    public static class GeoObjectExtentions
    {
        private const string TitleRuKey = "_titleRu";
        private const string TitleBeKey = "_titleBe";

        public static string TitleRu(this GeoObject osmObject) =>
            osmObject.Tags.GetValueOrDefault(TitleRuKey);

        public static string TitleBe(this GeoObject osmObject) =>
            osmObject.Tags.GetValueOrDefault(TitleBeKey);

        public static void SetTitleRu(this GeoObject osmObject, string value) =>
            osmObject.Tags[TitleRuKey] = value;
        
        public static void SetTitleBe(this GeoObject osmObject, string value) =>
            osmObject.Tags[TitleBeKey] = value;

        public static EntryData ToEntryDataAsPoint(this GeoObject geo, EntryType type) =>
            EntryHelper.GetData(
                geo.TitleRu(), geo.TitleBe(), type, geo.CenterCoords());
    }
}
