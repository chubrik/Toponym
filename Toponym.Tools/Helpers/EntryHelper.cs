using OsmDataKit.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Toponym.Core.Models;
using Toponym.Tools.Extensions;

namespace Toponym.Tools.Helpers
{
    public static class EntryHelper
    {
        public static EntryData GetData(
            string titleRu, string titleBe, EntryType type, IGeoCoords coords)
        {
            var screen = new List<ScreenCoords> { coords.ToScreen() };
            return GetData(titleRu, titleBe, type, coords, screen);
        }

        public static EntryData GetData(
            string titleRu, string titleBe, EntryType type, IGeoCoords coords, IEnumerable<ScreenCoords> screen)
        {
            Debug.Assert(titleRu != null);

            if (titleRu == null)
                throw new ArgumentNullException(nameof(titleRu));

            var latitude = (float)Math.Round(coords.Latitude, 4); // округление до ± 5,5 м
            var longitude = (float)Math.Round(coords.Longitude / 2, 4) * 2; // округление до ± 6,5 м

            return new EntryData
            {
                TitleRu = titleRu,
                TitleBe = titleBe,
                TitleEn = TextHelper.CyrillicToLatin(titleRu),
                Type = type,
                Coords = new[] { latitude, longitude },
                Screen = screen.Select(i => new[] { i.X, i.Y }).ToList()
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
