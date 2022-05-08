﻿using Kit;

namespace Toponym.Tools
{
    public static class Constants
    {
        private static readonly string _dataDir = PathHelper.Combine(Kit.Kit.BaseDirectory, "App_Data");

        // Source - https://download.geofabrik.de/europe/belarus.html
        public static readonly string Osm2022SourcePath = PathHelper.Combine(_dataDir, "belarus-220101.osm.pbf");
        public static readonly string Osm2019SourcePath = PathHelper.Combine(_dataDir, "belarus-190101.osm.pbf");
        public static readonly string Osm2017SourcePath = PathHelper.Combine(_dataDir, "belarus-170101.osm.pbf");

        public const string ProjectionDataPath = "projection.json";
        public const string BorderDataPath = "border.json";
        public const string BorderScreenDataPath = "border-screen.json";
        public const string BorderScreenHtmlPath = "border-screen.html";
        public const string PopulatedDataPath = "populated.json";
        public const string LocalitiesDataPath = "localities.json";
        public const string LakesDataPath = "lakes.json";
        public const string WatersDataPath = "waters.json";
        public const string RiversDataPath = "rivers.json";
        public const string ResultDataPath = "data.json";
        public const long OsmBorderRelationId = 59065; // https://www.openstreetmap.org/relation/59065
        public const long OsmBorderStartWayId = 22763008; // https://www.openstreetmap.org/way/22763008
        public const long OsmBorderStartNodeId = 146383660; // https://www.openstreetmap.org/node/146383660
    }
}
