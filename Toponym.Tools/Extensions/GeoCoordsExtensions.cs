using OsmDataKit;
using System;
using Toponym.Core.Models;
using Toponym.Tools.Helpers;
using Toponym.Tools.Services;

namespace Toponym.Tools.Extensions
{
    public static class GeoCoordsExtensions
    {
        private static readonly double _coeffPercent = ProjectionService.Data.Coeff * 100;

        public static ScreenCoords ToScreen(this IGeoCoords coords)
        {
            CoordsHelper.CalculateRaw(coords, out double rawX, out double rawY);
            var x = (float)Math.Round((rawX - ProjectionService.Data.MinRawX) * _coeffPercent, 2);
            var y = (float)Math.Round((rawY - ProjectionService.Data.MinRawY) * _coeffPercent, 2);
            return new ScreenCoords(x, y);
        }
    }
}
