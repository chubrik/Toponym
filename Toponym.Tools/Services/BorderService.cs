using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Tools.Extensions;

namespace Toponym.Tools.Services
{
    public class BorderService
    {
        public static IReadOnlyList<IGeoCoords> Build()
        {
            LogService.LogInfo("Load border");
            List<IGeoCoords> border;

            if (FileClient.Exists(Constants.BorderDataPath))
            {
                var readData = JsonFileClient.Read<List<double[]>>(Constants.BorderDataPath);
                border = readData.Select(i => new GeoCoords(i[0], i[1]) as IGeoCoords).ToList();
                LogService.LogInfo("Load border complete");
                return border;
            }

            border = new List<IGeoCoords>();
            var nodes = LoadNodes();
            var prevLatitude = 0d;
            var prevLongitude = 0d;

            foreach (var node in nodes)
            {
                if (node.Latitude.Equals(prevLatitude) && node.Longitude.Equals(prevLongitude))
                    continue;

                border.Add(node);
                prevLatitude = node.Latitude;
                prevLongitude = node.Longitude;
            }

            var saveData = border.Select(i => new[] { i.Latitude, i.Longitude }).ToList();
            JsonFileClient.Write(Constants.BorderDataPath, saveData);
            LogService.LogInfo("Load border complete");
            return border;
        }

        private static IEnumerable<NodeObject> LoadNodes()
        {
            var relation = GeoService.LoadRelation("border", Constants.OsmBorderRelationId);
            var ways = relation.Members.Where(i => i.Geo.Type == OsmGeoType.Way && i.Role == "outer").Select(i => (WayObject)i.Geo);

            LogService.Log("Sort nodes");
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

            LogService.Log("Sort nodes complete");
            return sortedNodes;
        }

        public static void BuildScreen(IEnumerable<IGeoCoords> border)
        {
            LogService.LogInfo("Build border screen coords");
            var borderScreen = new List<ScreenCoords>();
            var prevX = 0f;
            var prevY = 0f;

            foreach (var coords in border)
            {
                var screen = coords.ToScreen();

                if (Math.Abs(screen.X - prevX) < 0.1 && Math.Abs(screen.Y - prevY) < 0.1)
                    continue;

                borderScreen.Add(screen);
                prevX = screen.X;
                prevY = screen.Y;
            }

            var data = borderScreen.Select(i => new[] { i.X, i.Y }).ToList();
            JsonFileClient.Write(Constants.BorderScreenDataPath, data);

            const int width = 1000;
            var height = Math.Round(width * ProjectionService.Data.Ratio);
            var html = "";
            html += $"<div style=\"width: {width}px; height: {height}px; overflow-y: hidden; margin: 0 auto; padding: 6px 10px 10px 224px\">\n";
            html += $"    <svg width=\"{width}\" height=\"{width}\" viewBox=\"0 0 100 100\" preserveAspectRatio=\"none\" style=\"overflow: visible\">\n";
            html += "        <polygon stroke=\"black\" vector-effect=\"non-scaling-stroke\" stroke-width=\"0.35\" sstroke-linejoin=\"round\" fill=\"#fcfcfc\" points=\"";
            html += borderScreen.Select(i => $"{i.X.ToString(CultureInfo.InvariantCulture)},{i.Y.ToString(CultureInfo.InvariantCulture)}").Join(" ");
            html += "\" />\n    </svg>\n</div>\n";
            FileClient.Write(Constants.BorderScreenHtmlPath, html);
            LogService.LogInfo("Build border screen coords complete");
        }
    }
}
