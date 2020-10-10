using OsmDataKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Toponym.Tools
{
    public static class EntryHelper
    {
        public static EntryData GetData(
            string titleRu, string titleBe, EntryType type, Location location)
        {
            var screenPoints = new List<ScreenPoint> { location.ToScreen() };
            return GetData(titleRu, titleBe, type, location, screenPoints);
        }

        public static EntryData GetData(
            string titleRu, string titleBe, EntryType type, Location location, IEnumerable<ScreenPoint> screenPoints)
        {
            Debug.Assert(titleRu != null);

            if (titleRu == null)
                throw new ArgumentNullException(nameof(titleRu));

            var latitude = (float)Math.Round(location.Latitude, 4); // округление до ± 5,5 м
            var longitude = (float)Math.Round(location.Longitude / 2, 4) * 2; // округление до ± 6,5 м

            return new EntryData
            {
                TitleRu = titleRu,
                TitleBe = titleBe,
                TitleEn = TextHelper.CyrillicToLatin(titleRu),
                Type = type,
                Location = new[] { latitude, longitude },
                ScreenPoints = screenPoints.Select(i => new[] { i.X, i.Y }).ToList()
            };
        }

        public static void Validate(List<EntryData> data)
        {
            var badTitleRu = data.Where(i => !TextHelper.IsValidTitleRu(i.TitleRu)).ToList();
            var badTitleBe = data.Where(i => !TextHelper.IsValidTitleBe(i.TitleBe)).ToList();
            Debug.Assert(badTitleRu.Count == 0 || badTitleBe.Count == 0);

            if (badTitleRu.Count > 0 || badTitleBe.Count > 0)
                throw new InvalidOperationException();
        }
    }
}
