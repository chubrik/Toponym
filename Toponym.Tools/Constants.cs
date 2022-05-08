namespace Toponym.Tools
{
    public static class Constants
    {
        private const string _projectDir = @"..\..\..";
        private static readonly string _dataDir = Path.Combine(_projectDir, "App_Data");
        private static readonly string _workDir = Path.Combine(_projectDir, "$work");
        public static readonly string OsmCacheDir = Path.Combine(_workDir, "$osm-cache");

        // Source - https://download.geofabrik.de/europe/belarus.html
        public static readonly string Osm2022SourcePath = Path.Combine(_dataDir, "belarus-220101.osm.pbf");
        public static readonly string Osm2019SourcePath = Path.Combine(_dataDir, "belarus-190101.osm.pbf");
        public static readonly string Osm2017SourcePath = Path.Combine(_dataDir, "belarus-170101.osm.pbf");

        public static readonly string ProjectionDataPath = Path.Combine(_workDir, "projection.json");
        public static readonly string BorderDataPath = Path.Combine(_workDir, "border.json");
        public static readonly string BorderScreenDataPath = Path.Combine(_workDir, "border-screen.json");
        public static readonly string BorderScreenHtmlPath = Path.Combine(_workDir, "border-screen.html");
        public static readonly string PopulatedDataPath = Path.Combine(_workDir, "populated.json");
        public static readonly string LocalitiesDataPath = Path.Combine(_workDir, "localities.json");
        public static readonly string LakesDataPath = Path.Combine(_workDir, "lakes.json");
        public static readonly string WatersDataPath = Path.Combine(_workDir, "waters.json");
        public static readonly string RiversDataPath = Path.Combine(_workDir, "rivers.json");
        public static readonly string ResultDataPath = Path.Combine(_workDir, "data.json");

        public const long OsmBorderRelationId = 59065; // https://www.openstreetmap.org/relation/59065
        public const long OsmBorderStartWayId = 22763008; // https://www.openstreetmap.org/way/22763008
        public const long OsmBorderStartNodeId = 146383660; // https://www.openstreetmap.org/node/146383660
    }
}
