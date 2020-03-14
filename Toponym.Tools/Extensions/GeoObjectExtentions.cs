using OsmDataKit;
using System;
using System.Collections.Generic;

namespace Toponym.Tools
{
    public static class GeoObjectExtentions
    {
        private const string TitleRuKey = "_titleRu";
        private const string TitleBeKey = "_titleBe";
        private const string TypeKey = "_type";

        public static string TitleRu(this GeoObject geo) =>
            geo.Tags.GetValueOrDefault(TitleRuKey);

        public static string TitleBe(this GeoObject geo) =>
            geo.Tags.GetValueOrDefault(TitleBeKey);

        public static EntryType EntryType(this GeoObject geo) =>
            (EntryType)Enum.Parse(typeof(EntryType), geo.Tags[TypeKey]);

        public static void SetTitleRu(this GeoObject geo, string value)
        {
            if (geo.Tags == null)
                geo.Tags = new Dictionary<string, string>();

            geo.Tags[TitleRuKey] = value;
        }

        public static void SetTitleBe(this GeoObject geo, string value)
        {
            if (geo.Tags == null)
                geo.Tags = new Dictionary<string, string>();

            geo.Tags[TitleBeKey] = value;
        }

        public static void SetEntryType(this GeoObject geo, EntryType type)
        {
            if (geo.Tags == null)
                geo.Tags = new Dictionary<string, string>();

            geo.Tags[TypeKey] = type.ToString();
        }

        public static EntryData ToEntryDataAsPoint(this GeoObject geo, EntryType type) =>
            EntryHelper.GetData(geo.TitleRu(), geo.TitleBe(), type, geo.CenterLocation());
    }
}
