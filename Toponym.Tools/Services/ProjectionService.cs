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
                Data = FileClient.ReadObject<ProjectionData>(Constants.ProjectionDataPath);
                LogService.EndInfo("Build projection completed");
                return;
            }

            var borderLocations = BorderService.Build();
            var centerLocation = borderLocations.CenterLocation();

            Data.Lat0 = LocationHelper.DegToRad(centerLocation.Latitude);
            Data.Lng0 = LocationHelper.DegToRad(centerLocation.Longitude);
            Data.Tan0 = 1 / Math.Tan(Data.Lat0);
            Data.Sin0 = Math.Sin(Data.Lat0);

            var rawXs = new List<double>();
            var rawYs = new List<double>();

            foreach (var location in borderLocations)
            {
                LocationHelper.CalculateRaw(location, out double rawX, out double rawY);
                rawXs.Add(rawX);
                rawYs.Add(rawY);
            }

            Data.MinRawX = rawXs.Min();
            Data.MinRawY = rawYs.Min();
            Data.Coeff = 1 / (rawXs.Max() - Data.MinRawX);
            Data.Ratio = (rawYs.Max() - Data.MinRawY) * Data.Coeff;

            FileClient.WriteObject(Constants.ProjectionDataPath, Data);
            LogService.EndSuccess("Build projection completed");
            BorderService.BuildScreen(borderLocations);
        }
    }
}
