using OsmDataKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Toponym.Tools
{
    public static class WayObjectExtensions
    {
        public static EntryData ToEntryData(this WayObject way, EntryType type)
        {
            var firstLocation = way.Nodes.First().Location;
            var screenPoints = new List<ScreenPoint>();
            var prevX = 0f;
            var prevY = 0f;

            foreach (var wayNode in way.Nodes.Take(way.Nodes.Count - 1))
            {
                var screenPoint = wayNode.Location.ToScreen();

                if (Math.Abs(screenPoint.X - prevX) < 0.3 && Math.Abs(screenPoint.Y - prevY) < 0.3) // 3 пикселя
                    continue;

                screenPoints.Add(screenPoint);
                prevX = screenPoint.X;
                prevY = screenPoint.Y;
            }

            screenPoints.Add(way.Nodes.Last().Location.ToScreen());
            return EntryHelper.GetData(way.TitleRu(), way.TitleBe(), type, firstLocation, screenPoints);
        }
    }
}
