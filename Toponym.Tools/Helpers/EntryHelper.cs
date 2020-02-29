using OsmDataKit;
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
            string titleRu, string titleBe, EntryType type, IGeoPoint geoPoint)
        {
            var screenPoints = new List<ScreenPoint> { geoPoint.ToScreen() };
            return GetData(titleRu, titleBe, type, geoPoint, screenPoints);
        }

        public static EntryData GetData(
            string titleRu, string titleBe, EntryType type, IGeoPoint geoPoint, IEnumerable<ScreenPoint> screenPoints)
        {
            Debug.Assert(titleRu != null);

            if (titleRu == null)
                throw new ArgumentNullException(nameof(titleRu));

            var latitude = (float)Math.Round(geoPoint.Latitude, 4); // округление до ± 5,5 м
            var longitude = (float)Math.Round(geoPoint.Longitude / 2, 4) * 2; // округление до ± 6,5 м

            return new EntryData
            {
                TitleRu = titleRu,
                TitleBe = titleBe,
                TitleEn = TextHelper.CyrillicToLatin(titleRu),
                Type = type,
                GeoPoint = new[] { latitude, longitude },
                ScreenPoints = screenPoints.Select(i => new[] { i.X, i.Y }).ToList()
            };
        }

        public static void Validate(List<EntryData> data)
        {
            var badTitleRu = data.Where(i => !TextHelper.IsValidTitleRu(i.TitleRu)).ToList();
            var badTitleBe = data.Where(i => !TextHelper.IsValidTitleBe(i.TitleBe)).ToList();

            if (badTitleRu.Count > 0 || badTitleBe.Count > 0)
                throw new InvalidOperationException();
        }
    }
}
