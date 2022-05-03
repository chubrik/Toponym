namespace Toponym.Web
{
    public static class EntryExtensions
    {
        public static EntryTransport ToTransport(this Entry entry, Language language)
        {
            string title;

            switch (language)
            {
                case Language.Russian:
                    title = entry.TitleRu;
                    break;

                case Language.Belarusian:
                    title = entry.TitleBe;
                    break;

                case Language.English:
                    title = entry.TitleEn;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(language));
            }

            return new EntryTransport
            {
                Title = title,
                Type = entry.Type,
                GeoPoint = new[] { entry.GeoPoint.Latitude, entry.GeoPoint.Longitude },
                ScreenPoints = entry.ScreenPoints.Select(i => new[] { i.X, i.Y }).ToList()
            };
        }
    }
}
