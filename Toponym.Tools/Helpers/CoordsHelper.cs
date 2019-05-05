using OsmDataKit;
using System;
using Toponym.Tools.Services;

namespace Toponym.Tools.Helpers
{
    public static class CoordsHelper
    {
        public static double DegToRad(double deg) => deg / 180 * Math.PI;

        public static void CalculateRaw(IGeoCoords coords, out double rawX, out double rawY)
        {
            var deltaLat0 = ProjectionService.Data.Tan0 - (DegToRad(coords.Latitude) - ProjectionService.Data.Lat0);
            var deltaLong0 = ProjectionService.Data.Sin0 * (DegToRad(coords.Longitude) - ProjectionService.Data.Lng0);
            rawX = deltaLat0 * Math.Sin(deltaLong0);
            rawY = deltaLat0 * Math.Cos(deltaLong0) - ProjectionService.Data.Tan0;
        }
    }
}
