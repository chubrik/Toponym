using OsmDataKit;
using System;
using System.Collections.Generic;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Extensions
{
    public static class WayObjectExtensions
    {
        public static EntryData ToEntryData(this WayObject way, EntryType type)
        {
            var firstGeoPoint = way.Nodes.First() as IGeoPoint;
            var geoPoints = way.Nodes as IReadOnlyList<IGeoPoint>;
            var screenPoints = new List<ScreenPoint>();
            var prevX = 0f;
            var prevY = 0f;

            foreach (var geoPoint in geoPoints.Take(geoPoints.Count - 1))
            {
                var screenPoint = geoPoint.ToScreen();

                if (Math.Abs(screenPoint.X - prevX) < 0.3 && Math.Abs(screenPoint.Y - prevY) < 0.3) // 3 пикселя
                    continue;

                screenPoints.Add(screenPoint);
                prevX = screenPoint.X;
                prevY = screenPoint.Y;
            }

            screenPoints.Add(way.Nodes.Last().ToScreen());
            return EntryHelper.GetData(way.TitleRu(), way.TitleBe(), type, firstGeoPoint, screenPoints);
        }
    }
}
