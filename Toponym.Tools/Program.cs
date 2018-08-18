using Kit;
using System.Collections.Generic;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Tools.Extensions;
using Toponym.Tools.Helpers;
using Toponym.Tools.Services;

namespace Toponym.Tools
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Kit.Kit.Setup(baseDirectory: "../../..");
            //ConsoleClient.Setup(minLevel: LogLevel.Log);

            var arg = args.Length > 0 ? args[0] : "master";
            List<EntryData> data;

            switch (arg)
            {
                case "master":
                    LogService.LogInfo("Build master");
                    ProjectionService.Build();
                    var populated = PopulatedService.Build();
                    var lakes = LakeService.Build();
                    var waters = WaterService.Build();
                    var rivers = RiverService.Build();
                    data = populated.Concat(lakes).Concat(waters).Concat(rivers).ToSortedList();
                    EntryHelper.Validate(data);
                    JsonFileClient.Write(Constants.ResultDataPath, data);
                    var wrappedJson = FileClient.ReadText(Constants.ResultDataPath).Replace("},{", "},\r\n{");
                    FileClient.Write(Constants.ResultDataPath, wrappedJson);
                    LogService.LogInfo("Build master complete");
                    break;

                case "projection":
                    ProjectionService.Build();
                    break;

                case "populated":
                    ProjectionService.Build();
                    PopulatedService.Build();
                    break;

                case "lakes":
                    ProjectionService.Build();
                    LakeService.Build();
                    break;

                case "waters":
                    ProjectionService.Build();
                    WaterService.Build();
                    break;

                case "rivers":
                    LogService.LogInfo("Build rivers");
                    ProjectionService.Build();
                    data = RiverService.Build();
                    LogService.LogInfo("Build rivers complete");
                    break;

                case "ponds":
                    ProjectionService.Build();
                    PondService.Build();
                    break;

                default:
                    LogService.LogError($"Unknown command \"{arg}\"");
                    break;
            }
        }
    }
}
