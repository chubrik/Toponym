using Kit;
using OsmDataKit;
using OsmSharp;
using System.Text.RegularExpressions;

namespace Toponym.Tools
{
    public static class RiverService
    {
        public static List<EntryData> Build()
        {
            return LogService.InfoSuccess("Build rivers", () =>
            {
                //var response = OsmService.Get("rivers",
                //    i => (i.Tags.Contains("water", "river") ||
                //          i.Tags.Contains("waterway", "river") ||
                //          i.Tags.Contains("water", "riverbank") ||
                //          i.Tags.Contains("waterway", "riverbank") ||
                //          i.Tags.Contains("waterway", "stream")) &&
                //         GeoHelper.TitleRu(i) != null);

                var response = GeoService.Load(
                    "rivers",
                    i => i.Tags.Contains("waterway", "river") && GeoHelper.TitleRu(i) != null,
                    Constants.Osm2017SourcePath);

                var lostChernitsa = GeoService.LoadRelation(
                    "rivers-2022-chernitsa", 9191750, Constants.Osm2022SourcePath);

                FixNeman(response);

                var memberWays = response.RootRelations.Concat(new[] { lostChernitsa }).SelectMany(
                    relation =>
                    {
                        var members = NotNull(relation.Members).Where(i => i.Role == null || i.Role == "main_stream");

                        var geos = members.Select(
                            member =>
                            {
                                var geo = member.Geo;

                                if (geo.Type != OsmGeoType.Way)
                                    throw new InvalidOperationException();

                                if (geo.TitleRu() == null)
                                {
                                    var titleRu = relation.TitleRu();
                                    var titleBe = relation.TitleBe();

                                    if (titleRu != null)
                                        geo.SetTitleRu(titleRu);

                                    if (titleBe != null)
                                        geo.SetTitleBe(titleBe);
                                }

                                return (WayObject)geo;
                            });

                        return geos;
                    });

                var ways = response.RootWays.Concat(memberWays);
                var preFiltered = ways.Where(PreFilter).Select(PreFix);
                var rivers = GetMergedWays(preFiltered);
                var postFiltered = rivers.Where(PostFilter).Select(PostFix);
                var data = postFiltered.Select(i => i.ToEntryData(EntryType.River)).ToSortedList();
                FileHelper.WriteData(Constants.RiversDataPath, data);
                return data;
            });
        }

        private static void FixNeman(CompleteGeoObjects response)
        {
            var neman = response.RootRelations.Single(i => i.TitleRu() == "Неман");
            var members = NotNull(neman.Members).ToList();
            members.Remove(members.First(i => i.Geo.TitleRu() == "Неманец"));
            var nemanMembers = NotNull(neman.Members as List<RelationMemberObject>); // Hack
            nemanMembers.Clear();
            nemanMembers.AddRange(members);
        }

        private static bool PreFilter(WayObject way)
        {
            var normTitle = NotNull(way.TitleRu()).ToLower().Replace("ё", "е");

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
            var titleRu = NotNull(geo.TitleRu());
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

            if (titleRu == "Волма (сухое русло)")
                titleRu = "Волма";

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

            if (titleRu == "Сэрвач")
                titleRu = "Сервеч";

            if (titleBe != null)
                titleBe = Regex.Replace(titleBe, @"^р\. ", "");

            if (titleBe == "Бярэ́зіна")
                titleBe = "Бярэзіна";

            if (titleBe == "Тонкая Лучка (Турчанка)")
                titleBe = "Тонкая Лучка";

            if (geo.Id == 25064424) // Плиса
                titleRu = "Плисса";

            geo.SetTitleRu(titleRu);

            if (titleBe != null)
                geo.SetTitleBe(titleBe);

            return geo;
        }

        private static readonly HashSet<long> _badIds =
            new()
            {
                156664420, // Березина
                338382063,
                39539780,
                412711354,
                372010643, // Беседь
                330271771, // Бобр
                289495693,
                216766439, // Ботча
                25181970, // Зап. Буг
                152994779, // Вилия (петля)
                40220799, // Вилия (петля)
                25168235, // Вилия (петля)
                459024881, // Вилия (петля)
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
                200855163, // Случь (петля)
                255791504, // Сож
                258467157,
                340857436, // Ствига
                125940469, // Сула
                76488254, // Трубунка
                352282211, // Узлянка
                184870148, // Улла
                211918328, // Усса
                184582375, // Уша (кусочек, удалено)
                286938332, // Уша (петля)
                89912954,
                160034425, // Ушача
                368451997, // Цна
                407198034, // Щара
                164074534,
                259131488,
                50281450, // Янка
                445863217, // Дисна (петля)
                344073395, // Друть (кусочек, удалено)
                25176894, // Зельвянка (кусочек, удалено)
                25063100, // Маконь
                156349247, // Оболь (мелкий приток)
                437699714, // Овсянка (переделано в будущем)
                435304253, // Полота (боковой отрезок)
                119321368, // Случь (кусочек)
            };

        private static bool PostFilter(WayObject way)
        {
            if (_badIds.Contains(way.Id))
                return false;

            return true;
        }

        private static T PostFix<T>(T geo) where T : GeoObject => geo;

        private static List<WayObject> GetMergedWays(IEnumerable<WayObject> ways)
        {
            var groups = ways.GroupBy(i => NotNull(i.TitleRu()).ToLower().Replace('ё', 'е'));
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

        private static WayObject GetMergedWay(List<WayObject> waysLeft)
        {
            var first = waysLeft[0];
            var id = first.Id;
            var nodes = NotNull(first.Nodes).ToList();
            waysLeft.Remove(first);

            // Пристыкованные отрезки

            var repeat = true;

            while (repeat)
            {
                repeat = false;
                var usedWays = new List<WayObject>();

                foreach (var way in waysLeft)
                {
                    var wayNodes = NotNull(way.Nodes);

                    if (wayNodes[0].Id == nodes[^1].Id)
                    {
                        nodes.AddRange(wayNodes.Skip(1));
                        usedWays.Add(way);
                        repeat = true;
                    }
                    else if (wayNodes[^1].Id == nodes[0].Id)
                    {
                        nodes.InsertRange(0, wayNodes.Take(wayNodes.Count - 1));
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
                    var wayNodes = NotNull(way.Nodes);

                    if (IsNear(wayNodes[0], nodes[^1]))
                    {
                        nodes.AddRange(wayNodes);
                        usedWays.Add(way);
                        repeat = true;
                    }
                    else if (IsNear(wayNodes[^1], nodes[0]))
                    {
                        nodes.InsertRange(0, wayNodes);
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
