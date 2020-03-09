using Kit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Toponym.Tools
{
    public static class CompareService
    {
        public static void Build()
        {
            LogService.BeginInfo("Build compare");

            var oldData = JsonFileClient.Read<List<EntryData>>("../../Toponym.Site/App_Data/data.json");
            var newData = JsonFileClient.Read<List<EntryData>>(Constants.ResultDataPath);

            var oldPopulated = oldData.Where(i => (byte)i.Type >= 10 && (byte)i.Type < 20).ToList();
            var newPopulated = newData.Where(i => (byte)i.Type >= 10 && (byte)i.Type < 20).ToList();
            var oldLocalities = oldData.Where(i => (byte)i.Type == 30).ToList();
            var newLocalities = newData.Where(i => (byte)i.Type == 30).ToList();
            var oldWaters = oldData.Where(i => (byte)i.Type >= 20 && (byte)i.Type <= 22).ToList();
            var newWaters = newData.Where(i => (byte)i.Type >= 20 && (byte)i.Type <= 22).ToList();
            var oldRivers = oldData.Where(i => (byte)i.Type >= 23 && (byte)i.Type <= 24).ToList();
            var newRivers = newData.Where(i => (byte)i.Type >= 23 && (byte)i.Type <= 24).ToList();

            BuildByType("populated", oldPopulated, newPopulated);
            BuildByType("localities", oldLocalities, newLocalities);
            BuildByType("waters", oldWaters, newWaters);
            BuildByType("rivers", oldRivers, newRivers);

            LogService.EndSuccess("Build compare completed");
        }

        private static void BuildByType(string type, List<EntryData> oldData, List<EntryData> newData)
        {
            LogService.BeginInfo($"BuildByTyte '{type}'");
            var added = new List<EntryData>();
            var removed = new List<EntryData>();
            var changed = new List<(EntryData, EntryData)>();
            var same = new List<(EntryData, EntryData)>();

            LogService.BeginInfo($"{type}: Added, changed & same");

            foreach (var newEntry in newData)
            {
                var found = false;

                foreach (var oldEntry in oldData)
                {
                    if (Math.Abs(newEntry.ScreenPoints[0][0] - oldEntry.ScreenPoints[0][0]) > 0.1 ||
                        Math.Abs(newEntry.ScreenPoints[0][1] - oldEntry.ScreenPoints[0][1]) > 0.1)
                        continue;

                    found = true;

                    if (newEntry.Type == oldEntry.Type &&
                        newEntry.TitleRu == oldEntry.TitleRu &&
                        newEntry.TitleBe == oldEntry.TitleBe)
                        same.Add((oldEntry, newEntry));
                    else
                        changed.Add((oldEntry, newEntry));

                    break;
                }

                if (!found)
                    added.Add(newEntry);
            }

            LogService.EndSuccess("Build compare completed");
            LogService.BeginInfo("Removed");

            foreach (var oldEntry in oldData)
            {
                var found = false;

                foreach (var newEntry in newData)
                {
                    if (Math.Abs(newEntry.ScreenPoints[0][0] - oldEntry.ScreenPoints[0][0]) > 0.1 ||
                        Math.Abs(newEntry.ScreenPoints[0][1] - oldEntry.ScreenPoints[0][1]) > 0.1)
                        continue;

                    found = true;
                    break;
                }

                if (!found)
                    removed.Add(oldEntry);
            }

            LogService.EndSuccess("Removed completed");

            LogService.BeginInfo("Write html");
            HtmlHelper.Write($"{type}-added", added);
            HtmlHelper.Write($"{type}-removed", removed);
            HtmlHelper.Write($"{type}-changed", changed);
            LogService.EndSuccess("Write html completed");

            LogService.EndSuccess($"BuildByTyte '{type}' completed");
        }
    }
}
