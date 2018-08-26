using Toponym.Site.Models;

namespace Toponym.Site
{
    public class Constants
    {
        public const string DefaultHost = "toponim.by";
        public const string DataFileName = "data.json";
        public const int EntryCountLimit = 100000;

        public const EntryCategory AllEntryCategories = EntryCategory.Populated | EntryCategory.Water | EntryCategory.Locality;

        public static bool IsDebug =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
