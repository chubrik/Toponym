namespace Toponym.Site
{
    public class Constants
    {
        public const string DefaultHost = "toponim.by";
        public const int ItemCountLimit = 1000;

        public static bool IsDebug =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
