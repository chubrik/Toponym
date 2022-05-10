namespace Toponym.Tools;

using OsmDataKit;

public static class LocationHelper
{
    public static double DegToRad(double deg) => deg / 180 * Math.PI;

    public static void CalculateRaw(Location location, out double rawX, out double rawY)
    {
        var deltaLat0 = ProjectionService.Data.Tan0 - (DegToRad(location.Latitude) - ProjectionService.Data.Lat0);
        var deltaLong0 = ProjectionService.Data.Sin0 * (DegToRad(location.Longitude) - ProjectionService.Data.Lng0);
        rawX = deltaLat0 * Math.Sin(deltaLong0);
        rawY = deltaLat0 * Math.Cos(deltaLong0) - ProjectionService.Data.Tan0;
    }
}
