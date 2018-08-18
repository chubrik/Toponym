namespace Toponym.Site
{
    public class Constants
    {
        public const string DefaultHost = "toponim.by";
        public const string DataFileName = "data.json";
        public const int EntryCountLimit = 100000;

        public static bool IsDebug =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
