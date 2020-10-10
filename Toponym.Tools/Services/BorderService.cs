using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Toponym.Tools
{
    public static class BorderService
    {
        public static IReadOnlyList<Location> Build()
        {
            LogService.BeginInfo("Load border");
            List<Location> locations;

            if (FileClient.Exists(Constants.BorderDataPath))
            {
                var readData = FileClient.ReadObject<List<double[]>>(Constants.BorderDataPath);
                locations = readData.Select(i => new Location(i[0], i[1])).ToList();
                LogService.EndInfo("Load border completed");
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
            FileClient.WriteObject(Constants.BorderDataPath, saveData);
            LogService.EndSuccess("Load border completed");
            return locations;
        }

        private static IEnumerable<NodeObject> LoadNodes()
        {
            var relation = GeoService.LoadRelation("border", Constants.OsmBorderRelationId);
            var ways = relation.Members.Where(i => i.Geo.Type == OsmGeoType.Way && i.Role == "outer").Select(i => (WayObject)i.Geo);

            LogService.Begin("Sort nodes");
            var sortedNodes = new List<NodeObject>();
            var waysLeft = ways.ToList();
            var cursorNode = waysLeft.SelectMany(i => i.Nodes).First(i => i.Id == Constants.OsmBorderStartNodeId);
            var thisWay = waysLeft.Single(i => i.Id == Constants.OsmBorderStartWayId);

            while (true)
            {
                var thisNodes = thisWay.Nodes.ToList();

                if (thisNodes.Last().Id == cursorNode.Id)
                    thisNodes.Reverse();

                if (thisNodes.First().Id != cursorNode.Id)
                    throw new InvalidOperationException();

                cursorNode = thisNodes.Last();
                thisNodes.Remove(cursorNode);
                sortedNodes.AddRange(thisNodes);
                waysLeft.Remove(thisWay);

                if (waysLeft.Count == 0)
                    break;

                thisWay = waysLeft.Single(i => i.Nodes.Contains(cursorNode));
            }

            LogService.End("Sort nodes completed");
            return sortedNodes;
        }

        public static void BuildScreen(IEnumerable<Location> locations)
        {
            LogService.BeginInfo("Build border screen points");
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
            FileClient.WriteObject(Constants.BorderScreenDataPath, data);

            const int width = 1000;
            var height = Math.Round(width * ProjectionService.Data.Ratio);
            var html = "";
            html += $"<div style=\"width: {width}px; height: {height}px; overflow-y: hidden; margin: 0 auto; padding: 6px 10px 10px 224px\">\n";
            html += $"    <svg width=\"{width}\" height=\"{width}\" viewBox=\"0 0 100 100\" preserveAspectRatio=\"none\" style=\"overflow: visible\">\n";
            html += "        <polygon stroke=\"black\" vector-effect=\"non-scaling-stroke\" stroke-width=\"0.35\" sstroke-linejoin=\"round\" fill=\"#fcfcfc\" points=\"";
            html += screenPoints.Select(i => $"{i.X.ToString(CultureInfo.InvariantCulture)},{i.Y.ToString(CultureInfo.InvariantCulture)}").Join(" ");
            html += "\" />\n    </svg>\n</div>\n";
            FileClient.WriteText(Constants.BorderScreenHtmlPath, html);
            LogService.EndSuccess("Build border screen points completed");
        }
    }
}
