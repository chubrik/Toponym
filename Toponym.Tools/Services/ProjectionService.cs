using Kit;
using OsmDataKit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Toponym.Tools.Helpers;
using Toponym.Tools.Models;

namespace Toponym.Tools.Services
{
    public class ProjectionService
    {
        public static ProjectionData Data { get; private set; } = new ProjectionData();

        public static void Build()
        {
            LogService.LogInfo("Build projection");

            if (FileClient.Exists(Constants.ProjectionDataPath))
            {
                Data = JsonFileClient.Read<ProjectionData>(Constants.ProjectionDataPath);
                LogService.LogInfo("Build projection complete");
                return;
            }

            var border = BorderService.Build();
            var center = border.AverageCoords();

            Data.Lat0 = CoordsHelper.DegToRad(center.Latitude);
            Data.Long0 = CoordsHelper.DegToRad(center.Longitude);
            Data.Tan0 = 1 / Math.Tan(Data.Lat0);
            Data.Sin0 = Math.Sin(Data.Lat0);

            var rawXs = new List<double>();
            var rawYs = new List<double>();

            foreach (var coords in border)
            {
                CoordsHelper.CalculateRaw(coords, out double rawX, out double rawY);
                rawXs.Add(rawX);
                rawYs.Add(rawY);
            }

            Data.MinRawX = rawXs.Min();
            Data.MinRawY = rawYs.Min();
            Data.Coeff = 1 / (rawXs.Max() - Data.MinRawX);
            Data.Ratio = (rawYs.Max() - Data.MinRawY) * Data.Coeff;

            JsonFileClient.Write(Constants.ProjectionDataPath, Data);
            LogService.LogInfo("Build projection complete");

            BorderService.BuildScreen(border);
        }
    }
}
