using OsmDataKit;
using System;

namespace Toponym.Tools
{
    public static class LocationExtensions
    {
        private static readonly double _coeffPercent = ProjectionService.Data.Coeff * 100;

        public static ScreenPoint ToScreen(this Location location)
        {
            LocationHelper.CalculateRaw(location, out double rawX, out double rawY);
            var x = (float)Math.Round((rawX - ProjectionService.Data.MinRawX) * _coeffPercent, 2);
            var y = (float)Math.Round((rawY - ProjectionService.Data.MinRawY) * _coeffPercent, 2);
            return new ScreenPoint(x, y);
        }
    }
}
