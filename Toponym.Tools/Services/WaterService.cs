using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class WaterService
    {
        public static List<EntryData> Build()
        {
            LogService.BeginInfo("Build waters");

            var response = GeoService.Load(
                "waters",
                i => i.Tags.Contains("natural", "water") &&
                     !i.Tags.Contains("water", "river") &&
                     !i.Tags.Contains("waterway", "river") &&
                     !i.Tags.Contains("water", "riverbank") &&
                     !i.Tags.Contains("waterway", "riverbank") &&
                     !i.Tags.Contains("waterway", "stream") &&
                     GeoHelper.TitleRu(i) != null);

            var geos = response.RootObjects().Where(i => i.Type != OsmGeoType.Node).ToList();
            var rejected = geos.Where(i => !Filter(i)).ToList();
            HtmlHelper.Write("waters-rejected", rejected);

            var proceeded = geos.Where(Filter).Select(Proceed).ToList();
            HtmlHelper.Write("waters", proceeded.Where(i => i.EntryType() == EntryType.Water));
            HtmlHelper.Write("lakes", proceeded.Where(i => i.EntryType() == EntryType.Lake));
            HtmlHelper.Write("ponds", proceeded.Where(i => i.EntryType() == EntryType.Pond));

            var data = proceeded.Select(i => i.ToEntryDataAsPoint(i.EntryType())).ToSortedList();
            JsonFileClient.Write(Constants.WatersDataPath, data);
            LogService.EndSuccess("Build waters");
            return data;
        }

        #region Filter

        private static readonly HashSet<long> _rejectedWayIds = new HashSet<long> {
            25191008, // АБЗ
            25193438, // Бам
            385104711, // Брод. Проезд не возможен. затруднен.
            388081969, // В. Климовича
            765636581, // Велике Почаївське - за границей
            307536134, // Великоавцюковское
            258467162, // Горецкий Глухой затон
            258467159, // Горецкий затон
            261883838, // Гудшее чёрное - разделено лишь дорогой
            413907535, // Днепро-бугский канал / Дняпроўска-бугскі канал
            75658489, // ЗАБОЛОТСКОЕ
            406807941, // ЗКД
            244091389, // Заболоченная поляна
            330559710, // им. Абламейко
            336973595, // КСМ
            558544514, // купальня
            304193639, // купель
            132857383, // Лубянка - на реке
            25183775, // Медведки-1 - три озера рядом
            25183457, // Медведки-3
            299345978, // Москвич
            326213277, // мох
            169492251, // Мох
            249415390, // Новое
            294046824, // Новое Панское
            108584181, // НовоеПМК
            667487587, // о. Сосно - за границей
            30966250, // озеро Братилово - за границей
            25311717, // Озеро им.Пичеты
            85448013, // озеро КСМ
            169492252, // окопы
            159677475, // Плянта-1
            159677474, // Плянта-2
            363789541, // Плянта-3
            171993062, // ПМС
            700499384, // Пруд
            25185062, // Пруд-регулятор
            384798109, // пруды №14 и №15
            484382896, // Ратомский залив / Ратамская затока
            89833176, // Ров форта Е
            275176232, // Ровенскослободский / Ровенскаслабодская копанка
            122755536, // Сажелка
            484382897, // Спартаковский залив
            120325558, // Ставы
            25194001, // Старое
            223413670, // Старое
            375912097, // Старое русло Орессы
            108584578, // СтароеПМК
            287277438, // торфяник
            301773814 // У Шрека
        };

        private static readonly HashSet<long> _rejectedRelationIds = new HashSet<long> {
            9186041, // Гребной канал
            7757579, // залив Лисинский //todo
            7809472, // залив Старик //todo
            7825483, // Сириус - за границей
            7825484, // Сириус - за границей
            5927828 // Слёз / слёз
        };

        private static bool Filter(GeoObject geo)
        {
            if (geo.Type == OsmGeoType.Way && _rejectedWayIds.Contains(geo.Id))
            {
                //_rejectedWayIds.Remove(geo.Id);
                return false;
            }

            if (geo.Type == OsmGeoType.Relation && _rejectedRelationIds.Contains(geo.Id))
            {
                //_rejectedRelationIds.Remove(geo.Id);
                return false;
            }

            var normTitle = geo.TitleRu().ToLower().Replace("ё", "е");

            if (normTitle == "озеро" ||
                normTitle == "пруд" ||
                normTitle == "овраг" ||
                normTitle == "копань")
                return false;

            if (normTitle.StartsWith("№") ||
                normTitle.StartsWith("линза №") ||
                normTitle.StartsWith("пруд №") ||
                normTitle.StartsWith("затока ") || //todo
                normTitle.StartsWith("затон ")) //todo
                return false;

            if (normTitle.Contains("аквадром") ||
                normTitle.Contains("басейн") ||
                normTitle.Contains("бассейн") ||
                normTitle.Contains("вдхр") || //todo
                normTitle.Contains("водоем") ||
                normTitle.Contains("водонапор") ||
                normTitle.Contains("водоотвод") ||
                normTitle.Contains("водоподвод") ||
                normTitle.Contains("водохр.") || //todo
                normTitle.Contains("водохранилищ") || //todo
                normTitle.Contains("искусствен") ||
                normTitle.Contains("карьер") ||
                normTitle.Contains("колодец") ||
                normTitle.Contains("копанка") ||
                normTitle.Contains("кошанк") ||
                normTitle.Contains("мелиора") || // мелиоративный
                normTitle.Contains("набережн") ||
                normTitle.Contains("отст.") ||
                normTitle.Contains("отстойник") ||
                normTitle.Contains("пляж") ||
                normTitle.Contains("разлив") ||
                normTitle.Contains("рыбхоз") ||
                normTitle.Contains("скважин") ||
                normTitle.Contains("снаряд") ||
                normTitle.Contains("станция") ||
                normTitle.Contains("фантан") ||
                normTitle.Contains("фильтрац") ||
                normTitle.Contains("фонтан") ||
                normTitle.Contains("шламонакопитель") ||
                normTitle.Contains("шламосборник") ||
                normTitle.Contains("юбилей"))
                return false;

            return true;
        }

        #endregion

        private static GeoObject Proceed(GeoObject geo)
        {
            DetectType(geo);
            var titleRu = geo.TitleRu();
            var titleBe = geo.TitleBe();
            var id = geo.Id;

            if (geo.Type == OsmGeoType.Way)
            {
                if (id == 25183228) // старица Перелочь //todo старица
                    titleRu = "Перелочь";

                if (id == 25186164) // Глинское 2-е
                    titleRu = "Глинское";

                if (id == 25187251) // Омшарник / Імшарац
                    titleRu = "Имшарец";

                if (id == 25192023) // Каптару́ны
                    titleRu = "Каптаруны";

                if (id == 25192827) // Ильгиния / Вільчынія
                    titleBe = "Ільгінія";

                if (id == 25193821) // Сорочинское озеро / Сарочынскае возеро
                    titleBe = "Сарочынскае";

                if (id == 26300022) // Хмелевские пруды / Хмялеўскія сажалкі
                {
                    titleRu = "Хмелевские Пруды";
                    titleBe = "Хмялеўскія Сажалкі";
                    geo.SetEntryType(EntryType.Pond);
                }
                if (id == 70531432) // Бездежское  (Пырище)
                    titleRu = "Бездежское";

                if (id == 150888710) // старица Кукарская //todo старица
                    titleRu = "Кукарская";

                if (id == 227129731) // Пруд "Великие луки"
                    titleRu = "Великие Луки";

                if (id == 275526329) // Медведки-2 / Мядзвёдкі-2
                {
                    titleRu = "Медведки";
                    titleBe = "Мядзвёдкі";
                }
                if (id == 351078846) // Черные камни
                    titleRu = "Чёрные Камни";

                if (id == 374681272) // Спульный / Ўспольная
                    titleBe = "Успольная";

                if (id == 395711741) // оз Заборское
                    titleRu = "Заборское";

                if (id == 396057262) // залив Близно
                    titleRu = "Близно";

                if (id == 398734832) // пруд Пенчин
                    titleRu = "Пенчин";

                if (id == 406157241) // Колодное первое
                    titleRu = "Колодное";

                if (id == 413780610) // Сениченятка (Клянзин)
                    titleRu = "Сениченятка";

                if (id == 432915662) // Горелое болото / Гарэлае балато //todo болото
                {
                    titleRu = "Горелое Болото";
                    titleBe = "Гарэлае Балота";
                }
                if (id == 440616248) // пруд Великикй Млын
                    titleRu = "Великий Млын";

                if (id == 497064990) // Колодное второе
                    titleRu = "Колодное";

                if (id == 509488101) // пруд Жабы
                    titleRu = "Жабы";

                if (id == 559758200) // Пруд "Малая Своротва"
                    titleRu = "Малая Своротва";

                if (id == 626270849) // Глинское 1-е
                    titleRu = "Глинское";

                if (id == 677128905) // Пруд "Гута" //todo pond
                    titleRu = "Гута";

                if (id == 687500006) // Старица "Цыганский Заход" //todo старица
                    titleRu = "Цыганский Заход";
            }

            if (geo.Type == OsmGeoType.Relation)
            {
                if (id == 3879501) // Бол.Зеленец
                    titleRu = "Большой Зеленец";

                if (id == 5178855) // Мал. Зеленец
                    titleRu = "Малый Зеленец";

                if (id == 8220926) // Хмелевские пруды / Хмялеўскія сажалкі
                {
                    titleRu = "Хмелевские Пруды";
                    titleBe = "Хмялеўскія Сажалкі";
                    geo.SetEntryType(EntryType.Pond);
                }
                if (id == 8322297) // Мышечек (Спашера)
                    titleRu = "Мышечек";
            }

            if (!TextHelper.IsValidTitleRu(titleRu) ||
                !TextHelper.IsValidTitleBe(titleBe))
                throw new InvalidOperationException();

            geo.SetTitleRu(titleRu);
            geo.SetTitleBe(titleBe);
            return geo;
        }

        private static void DetectType(GeoObject geo)
        {
            var type = EntryType.Water;

            if (geo.Tags.Contains("water", "lake"))
                type = EntryType.Lake;

            else if (geo.Tags.Contains("water", "pond"))
                type = EntryType.Pond;

            geo.SetEntryType(type);

            var titleRu = geo.TitleRu();
            var titleBe = geo.TitleBe();

            // lake

            var newTitleRu = Regex.Replace(titleRu, @"^оз\.|(?<=^|\s)(озеро|озёра)(?=\s|$)", "", RegexOptions.IgnoreCase).Trim();
            var isMatched = newTitleRu != titleRu;
            titleRu = newTitleRu;

            if (titleBe != null)
            {
                var newTitleBe = Regex.Replace(titleBe, @"^воз\.|(?<=^|\s)(возера|азёры)(?=\s|$)", "", RegexOptions.IgnoreCase).Trim();
                isMatched = isMatched || newTitleBe != titleBe;
                titleBe = newTitleBe;
            }

            if (isMatched && geo.EntryType() != EntryType.Lake)
                geo.SetEntryType(EntryType.Lake);

            // pond

            newTitleRu = Regex.Replace(titleRu, @"(?<=^|\s)пруд(?=\s|$)", "", RegexOptions.IgnoreCase).Trim();
            isMatched = newTitleRu != titleRu;
            titleRu = newTitleRu;

            if (titleBe != null)
            {
                var newTitleBe = Regex.Replace(titleBe, @"(?<=^|\s)сажалка(?=\s|$)", "", RegexOptions.IgnoreCase).Trim();
                isMatched = isMatched || newTitleBe != titleBe;
                titleBe = newTitleBe;
            }

            if (isMatched && geo.EntryType() != EntryType.Pond)
                geo.SetEntryType(EntryType.Pond);

            geo.SetTitleRu(titleRu);
            geo.SetTitleBe(titleBe);
        }
    }
}
