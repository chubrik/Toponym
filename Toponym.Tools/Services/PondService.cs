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
            LogService.LogInfo("Build ponds");

            var response = GeoService.Load(
                "ponds-old",
                i => i.Tags.Contains("water", "pond") && GeoHelper.TitleRu(i) != null,
                Constants.OsmOldSourcePath);

            var wayData = response.Ways.Select(i => i.ToEntryDataAsPoint(EntryType.Pond));
            var relData = response.Relations.Select(i => i.ToEntryDataAsPoint(EntryType.Pond));
            var data = relData.Concat(wayData).ToSortedList();
            JsonFileClient.Write(Constants.PondsDataPath, data);
            LogService.LogInfo("Build ponds complete");
            return data;
        }
    }
}
