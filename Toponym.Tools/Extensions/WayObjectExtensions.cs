using OsmDataKit;

namespace Toponym.Tools
{
    public static class WayObjectExtensions
    {
        public static EntryData ToEntryData(this WayObject way, EntryType type)
        {
            var nodes = NotNull(way.Nodes);
            var firstLocation = nodes[0].Location;
            var screenPoints = new List<ScreenPoint>();
            var prevX = 0f;
            var prevY = 0f;

            foreach (var wayNode in nodes.Take(nodes.Count - 1))
            {
                var screenPoint = wayNode.Location.ToScreen();

                if (Math.Abs(screenPoint.X - prevX) < 0.3 && Math.Abs(screenPoint.Y - prevY) < 0.3) // 3 пикселя
                    continue;

                screenPoints.Add(screenPoint);
                prevX = screenPoint.X;
                prevY = screenPoint.Y;
            }

            screenPoints.Add(nodes[^1].Location.ToScreen());
            return EntryHelper.GetData(NotNull(way.TitleRu()), way.TitleBe(), type, firstLocation, screenPoints);
        }
    }
}
