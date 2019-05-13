using Kit;
using System.Collections.Generic;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Tools.Extensions;
using Toponym.Tools.Helpers;

namespace Toponym.Tools.Services
{
    public class PondService
    {
        public static List<EntryData> Build()
        {
            LogService.BeginInfo("Build ponds");

            var response = GeoService.Load(
                "ponds-old",
                i => i.Tags.Contains("water", "pond") && GeoHelper.TitleRu(i) != null,
                Constants.OsmOldSourcePath);

            var wayData = response.RootWays.Values.Select(i => i.ToEntryDataAsPoint(EntryType.Pond));
            var relData = response.RootRelations.Values.Select(i => i.ToEntryDataAsPoint(EntryType.Pond));
            var data = relData.Concat(wayData).ToSortedList();
            JsonFileClient.Write(Constants.PondsDataPath, data);
            LogService.EndSuccess("Build ponds completed");
            return data;
        }
    }
}
