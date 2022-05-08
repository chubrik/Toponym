using Kit;
using OsmDataKit;
using OsmSharp;
using System.Globalization;

namespace Toponym.Tools
{
    public static class BorderService
    {
        public static IReadOnlyList<Location> Build()
        {
            var logMessage = "Load border";
            LogService.BeginInfo(logMessage);
            List<Location> locations;

            if (File.Exists(Constants.BorderDataPath))
            {
                var readData = FileHelper.ReadData<IReadOnlyList<double[]>>(Constants.BorderDataPath);
                locations = readData.Select(i => new Location(i[0], i[1])).ToList();
                LogService.EndInfo(logMessage);
                return locations;
            }

            locations = new List<Location>();
            var nodes = LoadNodes();
            var prevLatitude = 0d;
            var prevLongitude = 0d;

            foreach (var node in nodes)
            {
                if (node.Latitude.Equals(prevLatitude) && node.Longitude.Equals(prevLongitude))
                    continue;

                locations.Add(node.Location);
                prevLatitude = node.Latitude;
                prevLongitude = node.Longitude;
            }

            var saveData = locations.Select(i => new[] { i.Latitude, i.Longitude }).ToList();
            FileHelper.WriteData(Constants.BorderDataPath, saveData);
            LogService.EndSuccess(logMessage);
            return locations;
        }

        private static IEnumerable<NodeObject> LoadNodes()
        {
            var relation = GeoService.LoadRelation("border", Constants.OsmBorderRelationId, Constants.Osm2022SourcePath);
            var ways = NotNull(relation.Members).Where(i => i.Geo.Type == OsmGeoType.Way && i.Role == "outer").Select(i => (WayObject)i.Geo);

            return LogService.Log("Sort nodes", () =>
            {
                var sortedNodes = new List<NodeObject>();
                var waysLeft = ways.ToList();
                var cursorNode = waysLeft.SelectMany(i => NotNull(i.Nodes)).First(i => i.Id == Constants.OsmBorderStartNodeId);
                var thisWay = waysLeft.Single(i => i.Id == Constants.OsmBorderStartWayId);

                while (true)
                {
                    var thisNodes = NotNull(thisWay.Nodes).ToList();

                    if (thisNodes[^1].Id == cursorNode.Id)
                        thisNodes.Reverse();

                    if (thisNodes[0].Id != cursorNode.Id)
                        throw new InvalidOperationException();

                    cursorNode = thisNodes[^1];
                    thisNodes.Remove(cursorNode);
                    sortedNodes.AddRange(thisNodes);
                    waysLeft.Remove(thisWay);

                    if (waysLeft.Count == 0)
                        break;

                    thisWay = waysLeft.Single(i => NotNull(i.Nodes).Contains(cursorNode));
                }

                return sortedNodes;
            });
        }

        public static void BuildScreen(IEnumerable<Location> locations)
        {
            LogService.InfoSuccess("Build border screen points", () =>
            {
                var screenPoints = new List<ScreenPoint>();
                var prevX = 0f;
                var prevY = 0f;

                foreach (var location in locations)
                {
                    var screenPoint = location.ToScreen();

                    if (Math.Abs(screenPoint.X - prevX) < 0.1 && Math.Abs(screenPoint.Y - prevY) < 0.1)
                        continue;

                    screenPoints.Add(screenPoint);
                    prevX = screenPoint.X;
                    prevY = screenPoint.Y;
                }

                var data = screenPoints.Select(i => new[] { i.X, i.Y }).ToList();
                FileHelper.WriteData(Constants.BorderScreenDataPath, data);

                const int width = 1000;
                var height = Math.Round(width * ProjectionService.Data.Ratio);
                var html = "";
                html += $"<div style=\"width: {width}px; height: {height}px; overflow-y: hidden; margin: 0 auto; padding: 6px 10px 10px 224px\">\n";
                html += $"    <svg width=\"{width}\" height=\"{width}\" viewBox=\"0 0 100 100\" preserveAspectRatio=\"none\" style=\"overflow: visible\">\n";
                html += "        <polygon stroke=\"black\" vector-effect=\"non-scaling-stroke\" stroke-width=\"0.35\" sstroke-linejoin=\"round\" fill=\"#fcfcfc\" points=\"";
                html += string.Join(' ', screenPoints.Select(i => $"{i.X.ToString(CultureInfo.InvariantCulture)},{i.Y.ToString(CultureInfo.InvariantCulture)}"));
                html += "\" />\n    </svg>\n</div>\n";
                File.WriteAllText(Constants.BorderScreenHtmlPath, html);
            });
        }
    }
}
