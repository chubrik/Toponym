using Kit;
using System.Linq;
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

            switch (arg)
            {
                case "master":
                    LogService.LogInfo("Build master");
                    ProjectionService.Build();
                    var populated = PopulatedService.Build();
                    var localities = LocalityService.Build();
                    var lakes = LakeService.Build();
                    var waters = WaterService.Build();
                    var rivers = RiverService.Build();
                    var data = populated.Concat(localities).Concat(lakes).Concat(waters).Concat(rivers).ToSortedList();
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

                case "localities":
                    ProjectionService.Build();
                    LocalityService.Build();
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
                    ProjectionService.Build();
                    RiverService.Build();
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