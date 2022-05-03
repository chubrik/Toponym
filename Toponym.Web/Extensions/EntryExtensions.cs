namespace Toponym.Web
{
    public static class EntryExtensions
    {
        public static EntryTransport ToTransport(this Entry entry, Language language)
        {
            var title = language switch
            {
                Language.Russian => entry.TitleRu,
                Language.Belarusian => NotNull(entry.TitleBe),
                Language.English => entry.TitleEn,
                _ => throw new ArgumentOutOfRangeException(nameof(language)),
            };

            return new EntryTransport(
                title: title,
                type: entry.Type,
                geoPoint: new[] { entry.GeoPoint.Latitude, entry.GeoPoint.Longitude },
                screenPoints: entry.ScreenPoints.Select(i => new[] { i.X, i.Y }).ToList());
        }
    }
}
