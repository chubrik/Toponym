﻿using Kit;
using OsmDataKit;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class LakeService
    {
        public static List<EntryData> Build()
        {
            return LogService.InfoSuccess("Build lakes", () =>
            {
                var response = GeoService.Load(
                    "lakes-old",
                    i => i.Tags.Contains("water", "lake") && GeoHelper.TitleRu(i) != null,
                    Constants.OsmOldSourcePath);

                LogService.Info("Filter & fix");
#if DEBUG
                var rejectedWays = response.RootWays.Where(i => !Filter(i)).OrderBy(i => i.TitleRu()).ToList();
                var rejectedRelations = response.RootRelations.Where(i => !Filter(i)).OrderBy(i => i.TitleRu()).ToList();
#endif
                var filteredWays = response.RootWays.Where(Filter).Select(Fix).OrderBy(i => i.TitleRu());
                var filteredRelations = response.RootRelations.Where(Filter).Select(Fix).OrderBy(i => i.TitleRu());
                var wayData = filteredWays.Select(i => i.ToEntryDataAsPoint(EntryType.Lake));
                var relData = filteredRelations.Select(i => i.ToEntryDataAsPoint(EntryType.Lake));
                var data = relData.Concat(wayData).ToSortedList();
                FileClient.WriteObject(Constants.LakesDataPath, data);
                return data;
            });
        }

        private static bool Filter(GeoObject geo)
        {
            var normTitle = geo.TitleRu().ToLower().Replace("ё", "е");

            if (normTitle.Contains("аэропорт") ||
                normTitle == "божье око" ||
                normTitle == "в. климовича" ||
                normTitle.Contains("вдхр") ||
                normTitle.Contains("водохранилище") ||
                normTitle.Contains("городск") ||
                normTitle.Contains("искусственн") ||
                normTitle == "озеро на репина" ||
                normTitle == "новоельнянское озеро" ||
                normTitle.Contains("плянта-") ||
                normTitle == "погост" ||
                normTitle == "старое возера" ||
                normTitle == "судовицкая лужа")
                return false;

            return true;
        }

        private static T Fix<T>(T geo) where T : GeoObject
        {
            var titleRu = Regex.Replace(geo.TitleRu(),
                @"(?<=^|\s)(оз\.?|озеро|озёра)(?=\s|$)|^оз\.", "", RegexOptions.IgnoreCase).Trim(' ', '"');

            var titleBe = geo.TitleBe();

            if (titleBe != null)
                titleBe = Regex.Replace(titleBe,
                    @"(?<=^|\s)(воз(\s?\.)?|возера|азёры)(?=\s|$)|^воз\.", "", RegexOptions.IgnoreCase).Trim(' ', '"');

            if (titleRu == "Бол. Зеленец")
                titleRu = "Большой Зеленец";

            if (titleRu == "Мал. Зеленец")
                titleRu = "Малый Зеленец";

            if (titleRu == "Бол. Комарино" || titleRu == "Бол.Комарино")
            {
                titleRu = "Большое Комарино";
                titleBe = "Вялікае Камарына";
            }

            if (titleRu == "Верхнее невицкое")
                titleRu = "Верхне-Невицкое";

            if (titleRu == "возера Бледнае")
                titleRu = "Бледное";

            if (titleRu == "Вымно (спортивное)")
            {
                titleRu = "Вымно";
                titleBe = "Вымна";
            }

            if (titleRu == "Гаранскае возера")
                titleRu = "Горанскае";

            if (titleRu == "Панское (Пробудилово)")
            {
                titleRu = "Пробудилово";
                titleBe = "Прабудзілава";
            }

            if (titleRu == "Попова криница")
                titleRu = "Попова Криница";

            if (titleBe == "Міхалінава (Грабеніцкае)")
                titleBe = "Грабеніцкае";

            if (titleRu == "Синяя лужа")
                titleRu = "Синяя Лужа";

            if (titleBe == "Сцернік (Бусоўня)")
                titleBe = "Сцернік";

            geo.SetTitleRu(titleRu);
            geo.SetTitleBe(titleBe);
            return geo;
        }
    }
}
