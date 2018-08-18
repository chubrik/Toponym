using OsmDataKit.Models;
using System.Collections.Generic;

namespace Toponym.Tools.Extensions
{
    public static class OsmObjectExtentions
    {
        private const string TitleRuKey = "TitleRu";
        private const string TitleBeKey = "TitleBe";

        public static string TitleRu(this OsmObject osmObject) =>
            osmObject.Data.GetValueOrDefault(TitleRuKey);

        public static string TitleBe(this OsmObject osmObject) =>
            osmObject.Data.GetValueOrDefault(TitleBeKey);

        public static void SetTitleRu(this OsmObject osmObject, string value) =>
            osmObject.Data[TitleRuKey] = value;
        
        public static void SetTitleBe(this OsmObject osmObject, string value) =>
            osmObject.Data[TitleBeKey] = value;
    }
}
