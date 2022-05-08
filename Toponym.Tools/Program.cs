using Kit;
using OsmDataKit;

namespace Toponym.Tools
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            OsmService.CacheDirectory = Constants.OsmCacheDir;
            var arg = args.Length > 0 ? args[0] : "all";

            switch (arg)
            {
                case "all":

                    LogService.InfoSuccess("Build all", () =>
                    {
                        ProjectionService.Build();
                        var populated = PopulatedService.Build();
                        var localities = LocalityService.Build();
                        var lakes = LakeService.Build();
                        var waters = WaterService.Build();
                        var rivers = RiverService.Build();
                        var data = populated.Concat(localities).Concat(lakes).Concat(waters).Concat(rivers).ToSortedList();
                        EntryHelper.Validate(data);
                        FileHelper.WriteData(Constants.ResultDataPath, data);

                        LogService.Info("Prettify data", () =>
                        {
                            var wrappedJson = File.ReadAllText(Constants.ResultDataPath).Replace("},{", "},\r\n{");
                            File.WriteAllText(Constants.ResultDataPath, wrappedJson);
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

                default:
                    LogService.Error($"Unknown command \"{arg}\"");
                    break;
            }
        }
    }
}
