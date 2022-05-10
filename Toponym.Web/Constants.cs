namespace Toponym.Web;

public static class Constants
{
    public const string DefaultHost = "toponim.by";
    public const string DataFileName = "data.json";
    public const int EntryCountLimit = 1000;

    public const EntryCategory AllEntryCategories = EntryCategory.Populated | EntryCategory.Water | EntryCategory.Locality;

    public static bool IsDebug =
#if DEBUG
        true;
#else
        false;
#endif
}
