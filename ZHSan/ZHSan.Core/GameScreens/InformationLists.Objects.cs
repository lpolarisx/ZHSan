using GameManager;
using GameObjects;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using PersonDetailPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using TroopDetailPlugin;
using static GameManager.MyraWidgets;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 右键菜单“信息”里的势力、城池、部队、人物、已故人物、编队、军区一览，以及右键菜单“年表”。
    /// 人物、城池点一行打开各自的详情窗口；其余在右侧详情栏显示简要信息，部队可以再打开完整详情
    /// </summary>
    internal static partial class InformationLists
    {
        private const int AbilityColumnWidth = 36;
        private const int FactionSwatchWidth = 10;
        private const int FactionSwatchHeight = 16;

        #region 势力

        public static void ShowFactions()
        {
            var factions = Session.Current.Scenario.Factions.Values.OrderBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<Faction>("势力", 120, (x, w) => CreateFactionCell(x), x => x.Name),
                new InfoColumn<Faction>("君主", 80, x => x.LeaderName, x => x.LeaderName),
                new InfoColumn<Faction>("城池", 50, x => x.ArchitectureCount.ToString(), x => x.ArchitectureCount),
                new InfoColumn<Faction>("武将", 50, x => x.PersonCount.ToString(), x => x.PersonCount),
                new InfoColumn<Faction>("兵力", 80, x => x.Army.ToString("N0"), x => x.Army),
                new InfoColumn<Faction>("声望", 70, x => x.Reputation.ToString("N0"), x => x.Reputation)
            };
            var filters = new[]
            {
                new InfoFilter<Faction>("玩家", x => Session.Current.Scenario.IsPlayer(x)),
                new InfoFilter<Faction>("电脑", x => !Session.Current.Scenario.IsPlayer(x))
            };
            InfoListDialog<Faction>.ShowWithDetail("势力", factions, columns, filters, FillFaction);
        }

        // 势力名前面一块势力颜色
        private static Widget CreateFactionCell(Faction faction)
        {
            var cell = new HorizontalStackPanel
            {
                Spacing = 8
            };
            cell.Widgets.Add(new Panel
            {
                Width = FactionSwatchWidth,
                Height = FactionSwatchHeight,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(faction.FactionColor)
            });
            var name = CreateLabel(faction.Name, 17, MyraSkin.TextPrimary);
            name.VerticalAlignment = VerticalAlignment.Center;
            cell.Widgets.Add(name);
            return cell;
        }

        /// <summary>
        /// 外交关系列表的势力格：势力还在时和势力列表一样带颜色块，已灭亡的只写名称
        /// </summary>
        internal static Widget CreateDiplomacyFactionCell(Faction faction, string name)
        {
            if (faction != null)
            {
                return CreateFactionCell(faction);
            }
            return CreateLabel(name, 17, MyraSkin.TextSecondary);
        }

        internal static void FillFaction(VerticalStackPanel panel, Faction faction, int width)
        {
            string caption = "势力" + (string.IsNullOrEmpty(faction.guanjuezifuchuan) ? "" : " · " + faction.guanjuezifuchuan)
                + (Session.Current.Scenario.IsPlayer(faction) ? " · 玩家" : "");
            AddDetailTop(panel, caption, faction.Name, MyraSkin.TextPrimary, width);
            panel.Widgets.Add(CreateSpacer(6));
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("城池", faction.ArchitectureCount.ToString(), MyraSkin.TextPrimary),
                ("武将", faction.PersonCount.ToString(), MyraSkin.TextPrimary),
                ("部队", faction.TroopCount.ToString(), MyraSkin.TextPrimary),
                ("兵力", faction.Army.ToString("N0"), Accent)
            }, width));
            panel.Widgets.Add(CreateSpacer(8));
            AddLinkRow(panel, "君主", faction.Leader?.Name, () => PersonDetailDialog.Show(faction.Leader, false), width);
            AddLinkRow(panel, "都城", faction.Capital?.Name, () => ArchitectureDetailDialog.Show(faction.Capital), width);
            panel.Widgets.Add(CreateStatGrid(new List<(string, string)>
            {
                ("声望", faction.Reputation.ToString("N0")),
                ("技巧点", faction.TotalTechniquePoint.ToString("N0")),
                ("资金", faction.Fund.ToString("N0")),
                ("粮草", faction.Food.ToString("N0")),
                ("人口", faction.Population.ToString("N0")),
                ("编队", faction.MilitaryCount.ToString())
            }, width));
            AddArchitectureLinks(panel, faction.Architectures, width);
            panel.Widgets.Add(CreateSpacer(10));
            panel.Widgets.Add(CreateLink("外交关系 ›", () => DiplomaticRelationListDialog.Show(faction)));
        }

        #endregion

        #region 城池

        /// <summary>
        /// 不传数据时显示全部城池；城池、势力右键菜单传入自己的城池和标题
        /// </summary>
        public static void ShowArchitectures(string title = "城池", IEnumerable<Architecture> items = null)
        {
            var architectures = (items ?? Session.Current.Scenario.Architectures.Values).OrderBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<Architecture>("名称", ArchitectureNameWidth, x => x.Name, x => x.Name, x => x.IsCapital ? Accent : MyraSkin.TextPrimary),
                new InfoColumn<Architecture>("势力", 80, x => x.FactionString, x => x.FactionString),
                new InfoColumn<Architecture>("规模", 44, x => x.JianzhuGuimo.ToString(), x => x.JianzhuGuimo),
                new InfoColumn<Architecture>("人口", 80, x => x.Population.ToString("N0"), x => x.Population),
                new InfoColumn<Architecture>("武将", 50, x => x.PersonCount.ToString(), x => x.PersonCount),
                new InfoColumn<Architecture>("兵力", 80, x => x.ArmyQuantity.ToString("N0"), x => x.ArmyQuantity)
            };
            var filters = architectures.Select(x => x.KindString).Distinct()
                .Select(kind => new InfoFilter<Architecture>(kind, x => x.KindString == kind));
            if (items != null)
            {
                filters = KeepUsefulFilters(filters, architectures);
            }
            InfoListDialog<Architecture>.Show(title, architectures, columns, filters, ArchitectureDetailDialog.Show,
                CreateArchitectureViews());
        }

        #endregion

        #region 部队

        /// <summary>
        /// 不传数据时显示全部部队；城池、势力右键菜单传入自己的部队和标题
        /// </summary>
        public static void ShowTroops(string title = "部队", IEnumerable<Troop> items = null)
        {
            var troops = (items ?? Session.Current.Scenario.Troops.Values).OrderBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<Troop>("部队", TroopNameWidth, x => x.DisplayName, x => x.DisplayName),
                new InfoColumn<Troop>("势力", 80, x => x.FactionString, x => x.FactionString),
                new InfoColumn<Troop>("兵种", 80, x => x.KindString, x => x.KindString),
                new InfoColumn<Troop>("兵力", 70, x => x.Quantity.ToString("N0"), x => x.Quantity),
                new InfoColumn<Troop>("士气", 50, x => x.Morale.ToString(), x => x.Morale),
                new InfoColumn<Troop>("战意", 50, x => x.Combativity.ToString(), x => x.Combativity)
            };
            InfoListDialog<Troop>.ShowWithDetail(title, troops, columns, null, FillTroop, CreateTroopViews());
        }

        // 部队简要信息：兵力、士气等卡片，势力、粮草、行动等，可以再打开部队详情
        private static void FillTroop(VerticalStackPanel panel, Troop troop, int width)
        {
            AddDetailTop(panel, "部队 · " + troop.KindString, troop.DisplayName, MyraSkin.TextPrimary, width);
            panel.Widgets.Add(CreateSpacer(6));
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("兵力", troop.Quantity.ToString("N0"), Accent),
                ("士气", troop.Morale.ToString(), MyraSkin.TextPrimary),
                ("战意", troop.Combativity.ToString(), MyraSkin.TextPrimary),
                ("攻击", troop.Offence.ToString(), MyraSkin.TextPrimary),
                ("防御", troop.Defence.ToString(), MyraSkin.TextPrimary)
            }, width));
            panel.Widgets.Add(CreateSpacer(8));
            panel.Widgets.Add(CreateStatList(new List<(string, string)>
            {
                ("势力", troop.FactionString),
                ("所属军区", troop.SectionString),
                ("人物数量", troop.PersonCount.ToString()),
                ("伤兵", troop.InjuryQuantity.ToString("N0")),
                ("粮草", troop.Food.ToString("N0")),
                ("口粮天数", troop.RationDaysString),
                ("行动力", troop.RealMovability.ToString()),
                ("战斗称号", troop.CombatTitleString),
                ("当前行动", troop.CurrentCombatAction),
                ("目标建筑", troop.TargetArchitectureString)
            }, width));
            panel.Widgets.Add(CreateSpacer(10));
            panel.Widgets.Add(CreateLink("部队详情 ›", () => TroopDetailDialog.Show(troop)));
        }

        #endregion

        #region 人物

        /// <summary>
        /// 不传数据时显示全部人物；城池、势力右键菜单传入自己的人物和标题
        /// </summary>
        /// <param name="showLocation">城池里的人物都在这座城，不显示“所在”</param>
        public static void ShowPersons(string title = "人物", IEnumerable<Person> items = null, bool showLocation = true)
        {
            var persons = (items ?? Session.Current.Scenario.AvailablePersons.Values).OrderBy(x => x.ID).ToList();
            var columns = new List<InfoColumn<Person>>
            {
                new InfoColumn<Person>("姓名", PersonNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Person>("势力", 80, GetFactionName, GetFactionName)
            };
            if (showLocation)
            {
                columns.Add(new InfoColumn<Person>("所在", 80, x => x.Location, x => x.Location));
            }
            columns.AddRange(CreateAbilityColumns());
            columns.AddRange(CreatePersonBasicColumns());
            IEnumerable<InfoFilter<Person>> filters = new[]
            {
                new InfoFilter<Person>("仕官", x => x.Status == PersonStatus.Normal || x.Status == PersonStatus.Moving),
                new InfoFilter<Person>("在野", x => x.Status == PersonStatus.NoFaction || x.Status == PersonStatus.NoFactionMoving),
                new InfoFilter<Person>("俘虏", x => x.Status == PersonStatus.Captive),
                new InfoFilter<Person>("后宫", x => x.Status == PersonStatus.Princess)
            };
            if (items != null)
            {
                filters = KeepUsefulFilters(filters, persons);
            }
            InfoListDialog<Person>.Show(title, persons, columns, filters, ShowPersonDetail,
                CreatePersonViews());
        }

        // 武、统、智、政、魅五列
        private static IEnumerable<InfoColumn<Person>> CreateAbilityColumns()
        {
            yield return new InfoColumn<Person>("武", AbilityColumnWidth, x => x.Strength.ToString(), x => x.Strength);
            yield return new InfoColumn<Person>("统", AbilityColumnWidth, x => x.Command.ToString(), x => x.Command);
            yield return new InfoColumn<Person>("智", AbilityColumnWidth, x => x.Intelligence.ToString(), x => x.Intelligence);
            yield return new InfoColumn<Person>("政", AbilityColumnWidth, x => x.Politics.ToString(), x => x.Politics);
            yield return new InfoColumn<Person>("魅", AbilityColumnWidth, x => x.Glamour.ToString(), x => x.Glamour);
        }

        #endregion

        #region 已故人物

        public static void ShowDeadPersons()
        {
            var persons = Session.Current.Scenario.GetDeadPersons().OrderBy(x => x.YearDead).ThenBy(x => x.ID).ToList();
            var columns = new List<InfoColumn<Person>>
            {
                new InfoColumn<Person>("姓名", PersonNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Person>("生卒", 100, x => x.YearBorn + " ～ " + x.YearDead, x => x.YearDead),
                new InfoColumn<Person>("享年", 50, x => GetLifeSpan(x).ToString(), x => GetLifeSpan(x)),
                new InfoColumn<Person>("死因", 80, x => x.DeadReason.ToString(), x => (int)x.DeadReason)
            };
            columns.AddRange(CreateAbilityColumns());
            var filters = Enum.GetValues(typeof(GameEnums.PersonDeadReason)).Cast<GameEnums.PersonDeadReason>()
                .Where(reason => persons.Any(x => x.DeadReason == reason))
                .Select(reason => new InfoFilter<Person>(reason.ToString(), x => x.DeadReason == reason));
            InfoListDialog<Person>.Show("已故人物", persons, columns, filters, ShowPersonDetail);
        }

        private static int GetLifeSpan(Person person)
        {
            return person.YearDead - person.YearBorn;
        }

        #endregion

        #region 编队

        /// <summary>
        /// 不传数据时显示全部编队；城池、势力右键菜单传入自己的编队和标题
        /// </summary>
        /// <param name="showLocation">城池里的编队都在这座城，不显示“所在”</param>
        public static void ShowMilitaries(string title = "编队", IEnumerable<Military> items = null, bool showLocation = true)
        {
            var militaries = (items ?? Session.Current.Scenario.Militaries.Values).OrderBy(x => x.ID).ToList();
            var columns = new List<InfoColumn<Military>>
            {
                new InfoColumn<Military>("编队", MilitaryNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Military>("兵种", 80, x => x.KindString, x => x.KindString),
                new InfoColumn<Military>("势力", 80, GetMilitaryFactionName, GetMilitaryFactionName)
            };
            if (showLocation)
            {
                columns.Add(new InfoColumn<Military>("所在", 80, x => x.LocationString, x => x.LocationString));
            }
            columns.Add(new InfoColumn<Military>("兵力", 64, x => x.Quantity.ToString("N0"), x => x.Quantity));
            columns.Add(new InfoColumn<Military>("士气", 44, x => x.Morale.ToString(), x => x.Morale));
            IEnumerable<InfoFilter<Military>> filters = new[]
            {
                new InfoFilter<Military>("城中", x => x.BelongedTroop == null),
                new InfoFilter<Military>("出征", x => x.BelongedTroop != null)
            };
            if (items != null)
            {
                filters = KeepUsefulFilters(filters, militaries);
            }
            InfoListDialog<Military>.ShowWithDetail(title, militaries, columns, filters, FillMilitary,
                CreateMilitaryViews(showLocation));
        }

        /// <summary>
        /// 正在运输的编队：不在城里也不在部队里，“所在”没有意义，换成起点、目的地和还要几天
        /// </summary>
        public static void ShowTransferingMilitaries(string title, IEnumerable<Military> items)
        {
            var militaries = items.OrderBy(x => x.ArrivingDays).ThenBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<Military>("编队", MilitaryNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Military>("兵种", 80, x => x.KindString, x => x.KindString),
                new InfoColumn<Military>("起点", 80, x => x.StartingArchitecture?.Name ?? "----", x => x.StartingArchitecture?.Name ?? ""),
                new InfoColumn<Military>("目的地", 80, x => x.TargetArchitecture?.Name ?? "----", x => x.TargetArchitecture?.Name ?? ""),
                new InfoColumn<Military>("剩余", 50, x => x.ArrivingDays + "天", x => x.ArrivingDays),
                new InfoColumn<Military>("兵力", 64, x => x.Quantity.ToString("N0"), x => x.Quantity)
            };
            InfoListDialog<Military>.ShowWithDetail(title, militaries, columns, null, FillMilitary,
                CreateMilitaryViews(true));
        }

        private static string GetMilitaryFactionName(Military military)
        {
            return military.BelongedFaction?.Name ?? "----";
        }

        // 上次看的是编队详情的哪个页签，换编队时停在同一页
        private static int militaryTab;

        // 编队详情：和兵种详情一样，顶部名称，几个主要数值的卡片，下面分状态、战斗、地形、升级四个页签。
        // 战斗页是编队实际的攻防（随规模和经验变），地形和升级沿用兵种的
        private static void FillMilitary(VerticalStackPanel panel, Military military, int width)
        {
            AddDetailTop(panel, "编队 · " + military.KindString + (military.BelongedTroop != null ? " · 出征" : ""), military.Name, MyraSkin.TextPrimary, width);
            panel.Widgets.Add(CreateSpacer(6));
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("兵力", military.Quantity.ToString("N0"), Accent),
                ("士气", military.Morale.ToString(), MyraSkin.TextPrimary),
                ("战意", military.Combativity.ToString(), MyraSkin.TextPrimary),
                ("攻击", military.Offence.ToString(), MyraSkin.TextPrimary),
                ("防御", military.Defence.ToString(), MyraSkin.TextPrimary)
            }, width));
            panel.Widgets.Add(CreateSpacer(8));
            var pages = new List<(string, Widget)>
            {
                ("状态", BuildMilitaryStatus(military, width)),
                ("战斗", BuildMilitaryCombat(military, width)),
                ("地形", BuildKindTerrain(military.Kind, width)),
                ("升级", BuildKindLevelUp(military.Kind, width, null, military.Experience))
            };
            AddTabs(panel, pages, width, militaryTab, x => militaryTab = x);
        }

        private static Widget BuildMilitaryStatus(Military military, int width)
        {
            var page = CreatePage();
            page.Widgets.Add(CreateStatList(new List<(string, string)>
            {
                ("势力", GetMilitaryFactionName(military)),
                ("统领", military.LeaderName),
                ("追随将领", military.FollowedLeaderName),
                ("经验", military.ExperienceWithLimit),
                ("规模", military.Scales + " 单位"),
                ("伤兵", military.InjuryQuantity.ToString("N0")),
                ("训练中", military.TrainingString),
                ("补充中", military.RecruitmentString),
                ("每日耗粮", military.FoodCostPerDay.ToString("N0")),
                ("疲累", military.Tiredness.ToString())
            }, width));
            if (military.BelongedTroop != null)
            {
                page.Widgets.Add(CreateSpacer(10));
                Troop troop = military.BelongedTroop;
                page.Widgets.Add(CreateLink("所在部队 ›", () => TroopDetailDialog.Show(troop)));
            }
            return page;
        }

        // 编队实际的攻防和范围，下面是兵种的攻击特性和兵种特性
        private static Widget BuildMilitaryCombat(Military military, int width)
        {
            MilitaryKind kind = military.Kind;
            var page = CreatePage();
            page.Widgets.Add(CreateStatGrid(new List<(string, string)>
            {
                ("攻击 / 防御", military.Offence + " / " + military.Defence),
                ("兵种攻防", kind.Offence + " / " + kind.Defence),
                ("行动力", military.Movability.ToString()),
                ("攻击范围", military.OffenceRadius.ToString()),
                ("计略范围", military.StratagemRadius.ToString()),
                ("视野范围", military.ViewRadius.ToString()),
                ("受火伤率", FormatRate(military.FireDamageRate)),
                ("建筑伤害率", FormatRate(military.ArchitectureDamageRate)),
                ("建筑反击率", FormatRate(military.ArchitectureCounterDamageRate)),
                ("编队上限", military.RecruitLimit.ToString())
            }, width));
            AddKindTraits(page, kind, width);
            return page;
        }

        #endregion

        #region 军区

        /// <summary>
        /// 不传数据时显示全部军区；城池、势力右键菜单传入自己的军区和标题
        /// </summary>
        public static void ShowSections(string title = "军区", IEnumerable<Section> items = null)
        {
            var sections = (items ?? Session.Current.Scenario.Sections.Values).OrderBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<Section>("军区", SectionNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Section>("势力", 80, GetSectionFactionName, GetSectionFactionName),
                new InfoColumn<Section>("委任", 100, x => x.AIDetailString, x => x.AIDetailString),
                new InfoColumn<Section>("城池", 50, x => x.ArchitectureCount.ToString(), x => x.ArchitectureCount),
                new InfoColumn<Section>("兵力", 80, x => x.Army.ToString("N0"), x => x.Army)
            };
            InfoListDialog<Section>.ShowWithDetail(title, sections, columns, null, FillSection,
                CreateSectionViews());
        }

        private static string GetSectionFactionName(Section section)
        {
            return section.BelongedFaction?.Name ?? "----";
        }

        private static void FillSection(VerticalStackPanel panel, Section section, int width)
        {
            AddDetailTop(panel, "军区 · " + GetSectionFactionName(section), section.Name, MyraSkin.TextPrimary, width);
            panel.Widgets.Add(CreateSpacer(6));
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("城池", section.ArchitectureCount.ToString(), MyraSkin.TextPrimary),
                ("武将", section.PersonCount.ToString(), MyraSkin.TextPrimary),
                ("部队", section.TroopCount.ToString(), MyraSkin.TextPrimary),
                ("兵力", section.Army.ToString("N0"), Accent)
            }, width));
            panel.Widgets.Add(CreateSpacer(8));
            panel.Widgets.Add(CreateStatList(new List<(string, string)>
            {
                ("委任模式", section.AIDetailString),
                ("委任目标", section.OrientationString),
                ("人口", section.Population.ToString("N0")),
                ("资金", section.Fund.ToString("N0")),
                ("粮草", section.Food.ToString("N0")),
                ("编队", section.MilitaryCount.ToString())
            }, width));
            AddArchitectureLinks(panel, section.Architectures, width);
        }

        #endregion

        #region 年表

        private const int YearTableContentWidth = 560;

        public static void ShowRecentYearTable()
        {
            var scenario = Session.Current.Scenario;
            ShowYearTable("年表 · 最近五年", scenario.YearTable.GetFactionYearTableRecentYears(scenario.CurrentPlayer, scenario.Date.Year - 5));
        }

        public static void ShowOwnFactionYearTable()
        {
            var scenario = Session.Current.Scenario;
            ShowYearTable("年表 · 自势力", scenario.YearTable.GetFactionYearTable(scenario.CurrentPlayer));
        }

        // 年表条目没有详情，不要详情栏；内容偏长时在格子里换行
        private static void ShowYearTable(string title, List<YearTableEntry> entries)
        {
            var columns = new[]
            {
                new InfoColumn<YearTableEntry>("日期", 130, x => x.Date.ToString(), x => x.Date.Year * 360 + x.Date.PassedDays),
                new InfoColumn<YearTableEntry>("势力", 150, (x, width) => CreateWrappedLabel(GetYearTableFactions(x), 15, MyraSkin.TextPrimary, width), GetYearTableFactions),
                new InfoColumn<YearTableEntry>("内容", YearTableContentWidth, (x, width) => CreateWrappedLabel(x.Content, 15, MyraSkin.TextPrimary, width))
            };
            // 年表条目没有详情，行不能点开
            InfoListDialog<YearTableEntry>.Show(title, entries, columns, null, null);
        }

        private static string GetYearTableFactions(YearTableEntry entry)
        {
            return string.Join("、", new[] { entry.FactionName1, entry.FactionName2, entry.FactionName3, entry.FactionName4, entry.FactionName5 }
                .Where(x => !string.IsNullOrEmpty(x)));
        }

        #endregion

        #region 共用

        private static void ShowPersonDetail(Person person)
        {
            PersonDetailDialog.Show(person, false);
        }
        /// <summary>
        /// 城池、势力里的列表只是一部分数据：没有一项符合的页签不显示；
        /// 只剩一个页签又包括了所有项时，和“全部”一样，页签都不要
        /// </summary>
        private static IEnumerable<InfoFilter<T>> KeepUsefulFilters<T>(IEnumerable<InfoFilter<T>> filters, List<T> items)
        {
            var useful = filters.Where(f => items.Any(f.Match)).ToList();
            return useful.Count == 1 && items.All(useful[0].Match) ? null : useful;
        }

        // 一行：左边名称，右边可点的名字；没有时写“----”
        private static void AddLinkRow(VerticalStackPanel panel, string name, string value, Action onClick, int width)
        {
            var row = new Panel
            {
                Width = width,
                Height = GridRowHeight
            };
            var label = CreateLabel(name, 14, MyraSkin.TextSecondary);
            label.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(label);
            Widget right;
            if (value == null)
            {
                right = CreateLabel("----", 15, MyraSkin.TextPrimary);
            }
            else
            {
                right = CreateLink(value + " ›", onClick);
            }
            right.HorizontalAlignment = HorizontalAlignment.Right;
            right.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(right);
            panel.Widgets.Add(WithDivider(row));
        }

        // 城池名字一个个排开，点了打开城池详情
        private static void AddArchitectureLinks(VerticalStackPanel panel, IEnumerable<Architecture> architectures, int width)
        {
            var list = architectures.ToList();
            panel.Widgets.Add(CreateDetailHeader("城池 · " + list.Count, width));
            if (list.Count == 0)
            {
                panel.Widgets.Add(CreateLabel("无", 14, MyraSkin.TextSecondary));
                return;
            }
            AddFlow(panel, list.Select(a => CreateLink(a.Name, () => ArchitectureDetailDialog.Show(a))), width);
        }

        // 人物名字一个个排开，点了打开人物详情
        private static void AddPersonLinks(VerticalStackPanel panel, string title, IEnumerable<Person> persons, int width)
        {
            var list = persons.ToList();
            panel.Widgets.Add(CreateDetailHeader(title + " · " + list.Count, width));
            if (list.Count == 0)
            {
                panel.Widgets.Add(CreateLabel("无", 14, MyraSkin.TextSecondary));
                return;
            }
            AddFlow(panel, list.Select(p => CreateLink(p.Name, () => PersonDetailDialog.Show(p, false))), width);
        }

        #endregion
    }
}
