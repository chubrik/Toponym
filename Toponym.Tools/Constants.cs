using Kit;

namespace Toponym.Tools
{
    public static class Constants
    {
        private const string RepositoryPath = "C:/Repositories/Toponym/";
        private static readonly string _projectPath = PathHelper.Combine(RepositoryPath, "Toponym.Tools");
        private static readonly string _dataPath = PathHelper.Combine(_projectPath, "App_Data");
        public static readonly string OsmOldSourcePath = PathHelper.Combine(_dataPath, "belarus-2016-06-11.osm.pbf");
        public static readonly string OsmNewSourcePath = PathHelper.Combine(_dataPath, "belarus-2018-08-10.osm.pbf");
        public const string ProjectionDataPath = "projection.json";
        public const string BorderDataPath = "border.json";
        public const string BorderScreenDataPath = "border-screen.json";
        public const string BorderScreenHtmlPath = "border-screen.html";
        public const string PopulatedDataPath = "populated.json";
        public const string LocalitiesDataPath = "localities.json";
        public const string LakesDataPath = "lakes.json";
        public const string PondsDataPath = "ponds.json";
        public const string WatersDataPath = "waters.json";
        public const string RiversDataPath = "rivers.json";
        public const string ResultDataPath = "data.json";
        public const long OsmBorderRelationId = 59065; // https://www.openstreetmap.org/relation/59065
        public const long OsmBorderStartWayId = 22763008; // https://www.openstreetmap.org/way/22763008
        public const long OsmBorderStartNodeId = 146383660; // https://www.openstreetmap.org/node/146383660
    }
}
