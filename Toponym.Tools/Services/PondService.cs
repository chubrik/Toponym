using Kit;
using System.Collections.Generic;
using System.Linq;

namespace Toponym.Tools
{
    public static class PondService
    {
        public static List<EntryData> Build()
        {
            LogService.BeginInfo("Build ponds");

            var response = GeoService.Load(
                "ponds-old",
                i => i.Tags.Contains("water", "pond") && GeoHelper.TitleRu(i) != null,
                Constants.OsmOldSourcePath);

            var wayData = response.RootWays.Select(i => i.ToEntryDataAsPoint(EntryType.Pond));
            var relData = response.RootRelations.Select(i => i.ToEntryDataAsPoint(EntryType.Pond));
            var data = relData.Concat(wayData).ToSortedList();
            FileClient.WriteObject(Constants.PondsDataPath, data);
            LogService.EndSuccess("Build ponds completed");
            return data;
        }
    }
}
