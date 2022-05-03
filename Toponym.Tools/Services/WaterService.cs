using Kit;
using OsmDataKit;
using OsmSharp;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class WaterService
    {
        public static List<EntryData> Build()
        {
            return LogService.InfoSuccess("Build waters", () =>
            {
                var response = GeoService.Load(
                    "waters-old",
                    i => i.Tags.Contains("natural", "water") &&
                         !i.Tags.Contains("water", "lake") &&
                         !i.Tags.Contains("water", "pond") &&
                         !i.Tags.Contains("water", "river") &&
                         !i.Tags.Contains("waterway", "river") &&
                         !i.Tags.Contains("water", "riverbank") &&
                         !i.Tags.Contains("waterway", "riverbank") &&
                         !i.Tags.Contains("waterway", "stream") &&
                         GeoHelper.TitleRu(i) != null,
                    Constants.OsmOldSourcePath);

                LogService.Info("Filter & fix");

                var rejectedWays = response.RootWays.Where(i => !Filter(i)).OrderBy(i => i.TitleRu()).ToList();
                var rejectedRelations = response.RootRelations.Where(i => !Filter(i)).OrderBy(i => i.TitleRu()).ToList();
                BuildHtmlLinks(rejectedWays, rejectedRelations);

                var filteredWays = response.RootWays.Where(Filter).Select(Fix).OrderBy(i => i.TitleRu());
                var filteredRelations = response.RootRelations.Where(Filter).Select(Fix).OrderBy(i => i.TitleRu());
                var wayData = filteredWays.Select(i => i.ToEntryDataAsPoint(EntryType.Lake));
                var relData = filteredRelations.Select(i => i.ToEntryDataAsPoint(EntryType.Lake));
                var data = wayData.Concat(relData).ToSortedList();
                FileClient.WriteObject(Constants.WatersDataPath, data);
                return data;
            });
        }

        #region Filter

        private static readonly long[] _rejectedWayIds = new long[] {
            25191008, // АБЗ
            25193438, // Бам
            25194033, // Бобр
            151808988, // Богинское - фрагмент
            307536134, // Великоавцюковское
            25183161, // Вилейское
            25537180, // Вилейское
            49263108, // Гребной канал
            261883838, // Гудшее чёрное - разделено лишь дорогой
            198430398, // жидкий
            75658489, // ЗАБОЛОТСКОЕ
            388243958, // Залив - затока
            396057262, // залив Близно
            398255454, // Калдыбаны - почти вплотную
            415079710, // копань
            330056492, // Копань
            330494171, // Копань
            335539823, // Копань
            335742261, // Копань
            366985758, // Котра - река
            25201923, // Краснослободское - фрагмент
            389490102, // Краснослободское - фрагмент
            25193443, // Кривая Речка
            25193799, // Кривая Речка
            132857383, // Лубянка - на реке
            243326788, // Мальчик на шаре - уже удалено
            25183775, // Медведки-1 - три озера рядом
            25183457, // Медведки-3
            397462064, // МОЕ
            299345978, // Москвич
            326213277, // мох
            169492251, // Мох
            249415390, // Новое
            294046824, // Новое Панское
            108584181, // НовоеПМК
            208687294, // оборок - все в одном городе
            242472293, // оборок
            242472294, // оборок
            30966250, // озеро Братилово - за границей
            86951112, // озеро с зайцами
            169492252, // окопы
            25191277, // Переново
            25187678, // Подкостёлок
            41927813, // Сириус - за границей
            25182584, // Став
            120325558, // Ставы
            25195045, // Стайки
            352280967, // Стайки
            25198605, // Старик
            25194001, // Старое
            223413670, // Старое
            108584578 // СтароеПМК
        };

        private static readonly long[] _rejectedRelationIds = new long[] {
            1722399, // Бобр - фрагмент реки Бобр
            5951571, // Краснослободское - фрагмент
            2245551 // протока Волотова
        };

        private static bool Filter(GeoObject geo)
        {
            var normTitle = geo.TitleRu().ToLower().Replace("ё", "е");

            if (normTitle == "бас." ||
                normTitle.Contains("басейн") ||
                normTitle.Contains("бассейн") ||
                normTitle == "болото" ||
                normTitle.Contains("вдхр") || //todo
                normTitle == "вода" ||
                normTitle.Contains("водоем") ||
                normTitle.Contains("водонапор") ||
                normTitle.Contains("водоотвод") ||
                normTitle.Contains("водоподвод") ||
                normTitle.Contains("водохр.") || //todo
                normTitle.Contains("водохранилищ") || //todo
                normTitle == "дамба" ||
                normTitle.Contains("днепр") ||
                normTitle.Contains("зап.двина") ||
                normTitle == "канава" ||
                normTitle == "канавы" ||
                normTitle.Contains("карьер") ||
                normTitle.Contains("колодец") ||
                normTitle.Contains("кошанк") ||
                normTitle.Contains("мелиора") || // мелиоративный
                normTitle.Contains("набережн") ||
                normTitle.Contains("неман") ||
                normTitle == "оборок" ||
                normTitle == "овраг" ||
                normTitle == "озеро" ||
                normTitle.Contains("отст.") ||
                normTitle.Contains("отстойник") ||
                normTitle.Contains("пляж") ||
                normTitle.Contains("припять") ||
                normTitle == "пруд" ||
                normTitle == "птичь" ||
                normTitle.Contains("разлив") ||
                normTitle.Contains("рыбхоз") ||
                normTitle.Contains("сажалк") ||
                normTitle.Contains("сажелк") ||
                normTitle.Contains("скважин") ||
                normTitle.Contains("снаряд") ||
                normTitle == "сож" ||
                normTitle == "ст.река" ||
                normTitle == "стар.птичь" ||
                normTitle == "трест" ||
                normTitle.Contains("фантан") ||
                normTitle.Contains("фильтрац") ||
                normTitle.Contains("фонтан") ||
                normTitle.Contains("юбилей"))
                return false;

            if (geo.Type == OsmGeoType.Way && _rejectedWayIds.Any(i => i == geo.Id))
                return false;

            if (geo.Type == OsmGeoType.Relation && _rejectedRelationIds.Any(i => i == geo.Id))
                return false;

            return true;
        }

        #endregion

        private static T Fix<T>(T geo) where T : GeoObject
        {
            var titleRu = Regex.Replace(geo.TitleRu(),
                @"(?<=^|\s)(озеро|озёра)(?=\s|$)|^о\.|^оз\.", "", RegexOptions.IgnoreCase).Trim(' ', '«', '»');

            var titleBe = geo.TitleBe();

            if (titleBe != null)
                titleBe = Regex.Replace(titleBe,
                    @"(?<=^|\s)(воз\.|возера)(?=\s|$)|^воз\.", "", RegexOptions.IgnoreCase).Trim();

            if (titleRu == "Бол.Сурвилишки")
                titleRu = "Большие Сурвилишки";

            if (titleRu == "Гудшее белое")
                titleRu = "Гудшее";

            if (titleRu == "Ельня-Большая")
                titleRu = "Ельня Большая";

            if (titleRu == "Ельня-Малая")
                titleRu = "Ельня Малая";

            if (titleRu == "Мал. Тучек")
                titleRu = "Малый Тучек";

            if (titleRu == "Медведки-2")
                titleRu = "Медведки";

            if (titleRu == "Омшарник")
                titleRu = "Имшарец";

            if (titleRu == "Бол.Живинское")
            {
                titleRu = "Большое Живинское";
                titleBe = "Вялікае Жывінскае";
            }

            if (titleRu == "Мал.Живинское")
            {
                titleRu = "Малое Живинское";
                titleBe = "Малое Жывінскае";
            }

            if (titleRu == "Мал.Комарино")
            {
                titleRu = "Малое Комарино";
                titleBe = "Малое Камарына";
            }

            if (titleRu == "Нов.Старик")
                titleRu = "Новый Старик";

            if (titleRu == "Стар.Забок")
                titleRu = "Старый Забок";

            if (titleRu == "ТОРФЯННИК")
                titleRu = "Вышковка";

            if (titleBe == "Убежа (Убежжа)")
                titleBe = "Убежа";

            if (titleBe == "Вільчынія")
                titleBe = "Ільгінія";

            if (titleRu[0] == titleRu.ToLower()[0])
                titleRu = titleRu.ToUpper()[0] + titleRu.Substring(1);

            geo.SetTitleRu(titleRu);
            geo.SetTitleBe(titleBe);
            return geo;
        }

        private static void BuildHtmlLinks(List<WayObject> ways, List<RelationObject> relations)
        {
            var html = "";

            for (var i = 0; i < relations.Count; i++)
                html += $"{i + 1}. <a href=\"https://" +
                        $"www.openstreetmap.org/relation/{relations[i].Id}\" " +
                        $"target=\"_blank\">{relations[i].TitleRu()}</a><br>\n";

            for (var i = 0; i < ways.Count; i++)
                html += $"{i + 1 + relations.Count}. <a href=\"https://" +
                        $"www.openstreetmap.org/way/{ways[i].Id}\" " +
                        $"target=\"_blank\">{ways[i].TitleRu()}</a><br>\n";

            FileClient.WriteText("waters-rejected-links.html", html);
        }
    }
}
