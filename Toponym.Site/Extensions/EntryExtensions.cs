using System;
using System.Linq;
using Toponym.Site.Models;

namespace Toponym.Site.Extensions
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
                Coords = new[] { entry.Coords.Latitude, entry.Coords.Longitude },
                Screen = entry.Screen.Select(i => new[] { i.X, i.Y }).ToList()
            };
        }
    }
}
