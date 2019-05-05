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
            var firstCoords = way.Nodes.First() as IGeoCoords;
            var coordsList = way.Nodes as IReadOnlyList<IGeoCoords>;
            var entryScreen = new List<ScreenCoords>();
            var prevX = 0f;
            var prevY = 0f;

            foreach (var coords in coordsList.Take(coordsList.Count - 1))
            {
                var screen = coords.ToScreen();

                if (Math.Abs(screen.X - prevX) < 0.3 && Math.Abs(screen.Y - prevY) < 0.3) // 3 пикселя
                    continue;

                entryScreen.Add(screen);
                prevX = screen.X;
                prevY = screen.Y;
            }

            entryScreen.Add(way.Nodes.Last().ToScreen());
            return EntryHelper.GetData(way.TitleRu(), way.TitleBe(), type, firstCoords, entryScreen);
        }
    }
}
