using Kit;
using OsmDataKit;
using OsmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class RiverService
    {
        public static List<EntryData> Build()
        {
            LogService.BeginInfo("Build rivers");

            //var response = OsmService.Get("rivers",
            //    i => (i.Tags.Contains("water", "river") ||
            //          i.Tags.Contains("waterway", "river") ||
            //          i.Tags.Contains("water", "riverbank") ||
            //          i.Tags.Contains("waterway", "riverbank") ||
            //          i.Tags.Contains("waterway", "stream")) &&
            //         GeoHelper.TitleRu(i) != null);

            var response = GeoService.Load(
                "rivers-old",
                i => i.Tags.Contains("waterway", "river") && GeoHelper.TitleRu(i) != null,
                Constants.OsmOldSourcePath);

            var rootRelations = response.RootRelations.Values;

            var allMemberWays = rootRelations.SelectMany(
                relation =>
                {
                    var members = relation.Members.Where(i => i.Role == "main_stream" || i.Role == "");

                    var memberWays = members.Select(
                        member =>
                        {
                            var memberWay = member.Geo as WayObject;

                            if (memberWay.Type != OsmGeoType.Way)
                                throw new InvalidOperationException();

                            if (memberWay.TitleRu() == null)
                            {
                                memberWay.SetTitleRu(relation.TitleRu());
                                memberWay.SetTitleBe(relation.TitleBe());
                            }

                            return memberWay;
                        });

                    return memberWays;
                });

            var ways = response.RootWays.Values.Concat(allMemberWays);
            var preFiltered = ways.Where(PreFilter).Select(PreFix);
            var rivers = GetMergedWays(preFiltered);
            var postFiltered = rivers.Where(PostFilter).Select(PostFix);
            var data = postFiltered.Select(i => i.ToEntryData(EntryType.River)).ToSortedList();
            JsonFileClient.Write(Constants.RiversDataPath, data);
            LogService.EndSuccess("Build rivers completed");
            return data;
        }

        private static bool PreFilter(WayObject way)
        {
            var normTitle = way.TitleRu().ToLower().Replace("ё", "е");

            if (normTitle.Contains("канал") ||
                normTitle.Contains("канава") ||
                normTitle.Contains("огород") ||
                normTitle.Contains("перекоп") ||
                normTitle.Contains("протока") ||
                normTitle.Contains("сажилка") ||
                normTitle.Contains("система") ||
                normTitle.Contains("старица"))
                return false;

            return true;
        }

        private static T PreFix<T>(T geo) where T : GeoObject
        {
            var titleRu = geo.TitleRu();
            var titleBe = geo.TitleBe();

            if (titleRu == "Западная Березина")
            {
                titleRu = "Березина";
                titleBe = "Бярэзіна";
            }

            if (titleRu == "земчанка")
            {
                titleRu = "Земчатка";
                titleBe = "Зямчатка";
            }

            if (titleRu == "Россия — Беларусь")
            {
                titleRu = "Нища";
                titleBe = "Нішча";
            }

            if (titleRu == "Вилия (Нярис)")
                titleRu = "Вилия";

            if (titleRu == "Гвозна Хвозьна")
                titleRu = "Гвозна";

            if (titleRu == "Западная Двина́")
                titleRu = "Западная Двина";

            if (titleRu == "Жолонь")
                titleRu = "Желонь";

            if (titleRu == "Лесовая Речка")
            {
                titleRu = "Льва";
                titleBe = "Льва";
            }

            if (titleRu == "Лесьна-Права")
            {
                titleRu = "Лесная Правая";
                titleBe = "Лясная Правая";
            }

            if (titleRu == "Мал. Речка")
                titleRu = "Малая Речка";

            if (titleRu == "Палата")
                titleRu = "Полота";

            if (titleRu == "Зилупе (Синюха)")
                titleRu = "Синюха";

            if (titleRu == "Сух. Поленница")
                titleRu = "Сухая Поленница";

            if (titleRu == "р. Хвощевка")
                titleRu = "Хвощевка";

            if (titleBe != null)
                titleBe = Regex.Replace(titleBe, @"^р\. ", "");

            if (titleBe == "Бярэ́зіна")
                titleBe = "Бярэзіна";

            if (titleBe == "Тонкая Лучка (Турчанка)")
                titleBe = "Тонкая Лучка";

            geo.SetTitleRu(titleRu);
            geo.SetTitleBe(titleBe);
            return geo;
        }

        private static readonly HashSet<long> _badWayIds =
            new HashSet<long> {
                156664420, // Березина
                338382063,
                39539780,
                412711354,
                372010643, // Беседь
                330271771, // Бобр
                289495693,
                216766439, // Ботча
                25181970, // Зап. Буг
                152994779, // Вилия
                40220799,
                25168235,
                390572312,
                230105489, // Вихра
                409817674, // Волка
                25066806, // Волма
                25175834, // Вязовка
                47328572, // Горынь
                404736018, // Друть
                83102018, // Ипуть
                117869286,
                25033596, // Колпита
                119307873, // Копаёвка
                193627557, // Корчевка
                258506188, // Котра
                327642750, // старица Лани
                379337559, // Млиновая речка (Староречье)
                55359977, // Мухавец
                25177675,
                328241390,
                410661130,
                194585031, // Неман
                354559896,
                223835045, // Нища
                118276511, // Оболь (удалено)
                370964803,
                250948661, // Ола
                116645626, // Ошмянка
                25064756, // Стар. Русло Ореса
                318345681, // Плиса
                318345680, // Плиса (сдвинутый дубликат)
                309530771, // Поня
                403216956, // Припять
                281327475,
                167797780,
                310066157, // Проня
                308845845, // Птичь
                119010992,
                25066850,
                292987510,
                405872934,
                403452018,
                310752500,
                25058479, // Стар. Птичь
                211731280, // р. Уса старыца
                394086133, // Рекотун
                389264115, // Ров
                376283706, // Старий рукав
                330828813, // Свислочь
                406947840,
                174689498,
                347628913, // Свольна
                153016693, // Сервеч
                167046595,
                25176483, // Сипа
                292830365, // Скрипица
                200855163, // Случь
                255791504, // Сож
                258467157,
                340857436, // Ствига
                125940469, // Сула
                76488254, // Трубунка
                352282211, // Узлянка
                184870148, // Улла
                211918328, // Усса
                184582375, // Уша
                286938332,
                89912954,
                160034425, // Ушача
                368451997, // Цна
                407198034, // Щара
                164074534,
                259131488,
                50281450, // Янка
                // 496202898 // Прушиновский ручей
            };

        private static bool PostFilter(WayObject way)
        {
            if (_badWayIds.Contains(way.Id))
                return false;

            return true;
        }

        private static T PostFix<T>(T geo) where T : GeoObject => geo;

        private static List<WayObject> GetMergedWays(IEnumerable<WayObject> ways)
        {
            var groups = ways.GroupBy(i => i.TitleRu().ToLower().Replace('ё', 'е'));
            var rivers = new List<WayObject>();

            foreach (var group in groups)
            {
                var waysLeft = group.ToList();

                while (waysLeft.Count > 0)
                {
                    var river = GetMergedWay(waysLeft);
                    rivers.Add(river);
                }
            }

            return rivers;
        }

        private static WayObject GetMergedWay(ICollection<WayObject> waysLeft)
        {
            var first = waysLeft.First();
            var id = first.Id;
            var nodes = first.Nodes.ToList();
            waysLeft.Remove(first);

            // Пристыкованные отрезки

            var repeat = true;

            while (repeat)
            {
                repeat = false;
                var usedWays = new List<WayObject>();

                foreach (var way in waysLeft)
                {
                    if (way.Nodes.First().Id == nodes.Last().Id)
                    {
                        nodes.AddRange(way.Nodes.Skip(1));
                        usedWays.Add(way);
                        repeat = true;
                    }
                    else if (way.Nodes.Last().Id == nodes.First().Id)
                    {
                        nodes.InsertRange(0, way.Nodes.Take(way.Nodes.Count - 1));
                        usedWays.Add(way);
                        id = way.Id;
                        repeat = true;
                    }
                }

                foreach (var usedWay in usedWays)
                    waysLeft.Remove(usedWay);

                if (waysLeft.Count == 0)
                    break;
            }

            // Близкие отрезки

            repeat = true;

            while (repeat)
            {
                repeat = false;
                var usedWays = new List<WayObject>();

                foreach (var way in waysLeft)
                {
                    if (IsNear(way.Nodes.First(), nodes.Last()))
                    {
                        nodes.AddRange(way.Nodes);
                        usedWays.Add(way);
                        repeat = true;
                    }
                    else if (IsNear(way.Nodes.Last(), nodes.First()))
                    {
                        nodes.InsertRange(0, way.Nodes);
                        usedWays.Add(way);
                        id = way.Id;
                        repeat = true;
                    }
                }

                foreach (var usedWay in usedWays)
                    waysLeft.Remove(usedWay);

                if (waysLeft.Count == 0)
                    break;
            }

            return new WayObject(id, nodes, first.Tags);
        }

        /// <summary>
        /// Ближе 5 км
        /// </summary>
        private static bool IsNear(NodeObject first, NodeObject second) =>
            Math.Abs(first.Latitude - second.Latitude) < 0.09 && // 5 / 55
            Math.Abs(first.Longitude - second.Longitude) < 0.075; // 5 / 65
    }
}
