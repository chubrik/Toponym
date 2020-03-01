using OsmDataKit;
using System;

namespace Toponym.Tools
{
    public static class GeoPointHelper
    {
        public static double DegToRad(double deg) => deg / 180 * Math.PI;

        public static void CalculateRaw(IGeoPoint geoPoint, out double rawX, out double rawY)
        {
            var deltaLat0 = ProjectionService.Data.Tan0 - (DegToRad(geoPoint.Latitude) - ProjectionService.Data.Lat0);
            var deltaLong0 = ProjectionService.Data.Sin0 * (DegToRad(geoPoint.Longitude) - ProjectionService.Data.Lng0);
            rawX = deltaLat0 * Math.Sin(deltaLong0);
            rawY = deltaLat0 * Math.Cos(deltaLong0) - ProjectionService.Data.Tan0;
        }
    }
}
