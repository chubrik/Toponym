using Kit;
using OsmDataKit;
using System.Linq;

namespace Toponym.Tools
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Kit.Kit.Setup();
            OsmService.CacheDirectory = PathHelper.Combine(Kit.Kit.BaseDirectory, "$work/$osm-cache");
            var arg = args.Length > 0 ? args[0] : "master";

            switch (arg)
            {
                case "master":
                    LogService.InfoSuccess("Build master", () =>
                    {
                        ProjectionService.Build();
                        var populated = PopulatedService.Build();
                        var localities = LocalityService.Build();
                        var waters = WaterService.Build();
                        var rivers = RiverService.Build();
                        var data = populated.Concat(localities).Concat(waters).Concat(rivers).ToSortedList();
                        EntryHelper.Validate(data);
                        FileClient.WriteObject(Constants.ResultDataPath, data);

                        LogService.Log("Prettify data", () => {
                            var wrappedJson = FileClient.ReadText(Constants.ResultDataPath).Replace("},{", "},\r\n{");
                            FileClient.WriteText(Constants.ResultDataPath, wrappedJson);
                        });
                    });

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

                case "waters":
                    ProjectionService.Build();
                    WaterService.Build();
                    break;

                case "rivers":
                    ProjectionService.Build();
                    RiverService.Build();
                    break;

                case "compare":
                    CompareService.Build();
                    break;

                default:
                    LogService.Error($"Unknown command \"{arg}\"");
                    break;
            }
        }
    }
}
