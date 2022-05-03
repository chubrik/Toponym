using Kit;
using OsmDataKit;

namespace Toponym.Tools
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            OsmService.CacheDirectory = PathHelper.Combine(Kit.Kit.BaseDirectory, Kit.Kit.WorkingDirectory, "$osm-cache");
            var arg = args.Length > 0 ? args[0] : "master";

            switch (arg)
            {
                case "master":

                    LogService.InfoSuccess("Build master", () =>
                    {
                        ProjectionService.Build();
                        var populated = PopulatedService.Build();
                        var localities = LocalityService.Build();
                        var lakes = LakeService.Build();
                        var waters = WaterService.Build();
                        var rivers = RiverService.Build();
                        var data = populated.Concat(localities).Concat(lakes).Concat(waters).Concat(rivers).ToSortedList();
                        EntryHelper.Validate(data);
                        FileClient.WriteObject(Constants.ResultDataPath, data);

                        LogService.Info("Prettify data", () =>
                        {
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
                    LogService.Error($"Unknown command \"{arg}\"");
                    break;
            }
        }
    }
}
