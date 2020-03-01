using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Toponym.Tools
{
    public class BorderService
    {
        public static IReadOnlyList<IGeoPoint> Build()
        {
            LogService.BeginInfo("Load border");
            List<IGeoPoint> geoPoints;

            if (FileClient.Exists(Constants.BorderDataPath))
            {
                var readData = JsonFileClient.Read<List<double[]>>(Constants.BorderDataPath);
                geoPoints = readData.Select(i => new GeoPoint(i[0], i[1]) as IGeoPoint).ToList();
                LogService.EndInfo("Load border completed");
                return geoPoints;
            }

            geoPoints = new List<IGeoPoint>();
            var nodes = LoadNodes();
            var prevLatitude = 0d;
            var prevLongitude = 0d;

            foreach (var node in nodes)
            {
                if (node.Latitude.Equals(prevLatitude) && node.Longitude.Equals(prevLongitude))
                    continue;

                geoPoints.Add(node);
                prevLatitude = node.Latitude;
                prevLongitude = node.Longitude;
            }

            var saveData = geoPoints.Select(i => new[] { i.Latitude, i.Longitude }).ToList();
            JsonFileClient.Write(Constants.BorderDataPath, saveData);
            LogService.EndSuccess("Load border completed");
            return geoPoints;
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

        public static void BuildScreen(IEnumerable<IGeoPoint> geoPoints)
        {
            LogService.BeginInfo("Build border screen points");
            var screenPoints = new List<ScreenPoint>();
            var prevX = 0f;
            var prevY = 0f;

            foreach (var geoPoint in geoPoints)
            {
                var screenPoint = geoPoint.ToScreen();

                if (Math.Abs(screenPoint.X - prevX) < 0.1 && Math.Abs(screenPoint.Y - prevY) < 0.1)
                    continue;

                screenPoints.Add(screenPoint);
                prevX = screenPoint.X;
                prevY = screenPoint.Y;
            }

            var data = screenPoints.Select(i => new[] { i.X, i.Y }).ToList();
            JsonFileClient.Write(Constants.BorderScreenDataPath, data);

            const int width = 1000;
            var height = Math.Round(width * ProjectionService.Data.Ratio);
            var html = "";
            html += $"<div style=\"width: {width}px; height: {height}px; overflow-y: hidden; margin: 0 auto; padding: 6px 10px 10px 224px\">\n";
            html += $"    <svg width=\"{width}\" height=\"{width}\" viewBox=\"0 0 100 100\" preserveAspectRatio=\"none\" style=\"overflow: visible\">\n";
            html += "        <polygon stroke=\"black\" vector-effect=\"non-scaling-stroke\" stroke-width=\"0.35\" sstroke-linejoin=\"round\" fill=\"#fcfcfc\" points=\"";
            html += screenPoints.Select(i => $"{i.X.ToString(CultureInfo.InvariantCulture)},{i.Y.ToString(CultureInfo.InvariantCulture)}").Join(" ");
            html += "\" />\n    </svg>\n</div>\n";
            FileClient.Write(Constants.BorderScreenHtmlPath, html);
            LogService.EndSuccess("Build border screen points completed");
        }
    }
}
