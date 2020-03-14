using Kit;
using OsmDataKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Toponym.Tools
{
    public static class ProjectionService
    {
        public static ProjectionData Data { get; private set; } = new ProjectionData();

        public static void Build()
        {
            LogService.BeginInfo("Build projection");

            if (FileClient.Exists(Constants.ProjectionDataPath))
            {
                Data = JsonFileClient.Read<ProjectionData>(Constants.ProjectionDataPath);
                LogService.EndInfo("Build projection completed");
                return;
            }

            var borderGeoPoints = BorderService.Build();
            var centerGeoPoint = borderGeoPoints.CenterLocation();

            Data.Lat0 = GeoPointHelper.DegToRad(centerGeoPoint.Latitude);
            Data.Lng0 = GeoPointHelper.DegToRad(centerGeoPoint.Longitude);
            Data.Tan0 = 1 / Math.Tan(Data.Lat0);
            Data.Sin0 = Math.Sin(Data.Lat0);

            var rawXs = new List<double>();
            var rawYs = new List<double>();

            foreach (var geoPoint in borderGeoPoints)
            {
                GeoPointHelper.CalculateRaw(geoPoint, out double rawX, out double rawY);
                rawXs.Add(rawX);
                rawYs.Add(rawY);
            }

            Data.MinRawX = rawXs.Min();
            Data.MinRawY = rawYs.Min();
            Data.Coeff = 1 / (rawXs.Max() - Data.MinRawX);
            Data.Ratio = (rawYs.Max() - Data.MinRawY) * Data.Coeff;

            JsonFileClient.Write(Constants.ProjectionDataPath, Data);
            LogService.EndSuccess("Build projection completed");
            BorderService.BuildScreen(borderGeoPoints);
        }
    }
}
