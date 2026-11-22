using Extensions;
using FontStashSharp;
using FontStashSharp.RichText;
using GameEnums;
using GameManager;
using GameObjects;
using GameObjects.ArchitectureDetail;
using GameObjects.Conditions;
using GameObjects.Influences;
using GameObjects.MapDetail;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using Microsoft.Xna.Framework;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.UI;
using PersonDetailPlugin;
using Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static GameManager.MyraWidgets;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 右键菜单“信息”里的称号、技能、特技、兵种、宝物一览：定义各自的列、筛选和详情内容，用InfoListDialog显示
    /// </summary>
    internal static partial class InformationLists
    {
        private const int StatRowHeight = 24;
        private const int StatLabelWidth = 110;
        private const int KindCardHeight = 58;
        private const int KindCardSpacing = 6;
        private const int TabHeight = 36;
        private const int GridRowHeight = 34;
        private const int GridColumnGap = 16;
        private const int TerrainGroupHeaderHeight = 28;
        private const int TerrainCardsPerRow = 4;
        private const int TerrainCardSpacing = 8;
        private const int TerrainCardHeight = 76;
        private const int RouteNodeHeight = 50;
        private const int RouteNodeSpacing = 10;
        private const int RouteGap = 22;
        // 设施图按城池详情里的165×91等比缩小
        private const int FacilityPictureWidth = 110;
        private const int FacilityPictureHeight = 61;
        private const int TreasurePictureSize = 96;
        private const int LinkHeight = 26;
        private const int FlowSpacing = 6;

        private const int ChipFontSize = 14;
        private const int ChipPaddingX = 9;
        private const int ChipHeight = 26;
        private const int ChipSpacing = 6;
        // 放不下的效果在末尾写“+N”，给它留的宽度
        private const int ChipMoreWidth = 30;
        private const int EffectRowPadding = 9;

        private static readonly Color Accent = MyraSkin.TextHighlight;
        private static readonly Color ChipColor = new Color(30, 44, 54);
        private static readonly Color DividerColor = LineColor * 0.45f;

        private static readonly string[] AbilityNames = { "武力", "统率", "智力", "政治", "魅力" };

        // 限定生效时机的影响在行里写成名称后面的小标签：仅君主、仅部队主将、满足条件、限定兵种类型、限定兵种
        private static readonly Dictionary<int, string> RestrictionTags = new Dictionary<int, string>
        {
            { 280, "君主" },
            { 281, "主将" },
            { 285, "条件" },
            { 290, "兵种" },
            { 300, "兵种" }
        };

        private static IEnumerable<Person> AvailablePersons => Session.Current.Scenario.AvailablePersons.Values;

        #region 称号

        public static void ShowTitles()
        {
            var titles = Session.Current.Scenario.GameCommonData.AllTitles.Values.OrderBy(x => x.KindId).ThenBy(x => x.Level).ToList();
            var columns = new[]
            {
                new InfoColumn<Title>("名称", 180, (x, w) => CreateNameCell(x.Name, PersonDetailDialog.GetTitleColor(x), x.Influences.Values), x => x.Name),
                new InfoColumn<Title>("等级", 50, x => x.Level.ToString(), x => x.Level),
                new InfoColumn<Title>("效果", 0, (x, w) => CreateEffectChips(x.Influences.Values, w), x => x.InfluenceCount)
            };
            var filters = Session.Current.Scenario.GameCommonData.AllTitleKinds.Values.OrderBy(x => x.ID)
                .Select(kind => new InfoFilter<Title>(kind.Name, x => x.KindId == kind.ID));
            InfoListDialog<Title>.ShowWithDetail("称号", titles, columns, filters, FillTitle);
        }

        private static void FillTitle(VerticalStackPanel panel, Title title, int width)
        {
            var notes = new List<string> { "称号", title.KindName };
            if (title.Combat)
            {
                notes.Add("战斗");
            }
            AddDetailTop(panel, string.Join(" · ", notes), title.Name, PersonDetailDialog.GetTitleColor(title), width);
            AddStat(panel, "等级", title.Level.ToString(), width);
            if (title.FundForHolder > 0)
            {
                AddStat(panel, "俸金", title.FundForHolder + " / 月", width);
            }
            if (title.ManualAward)
            {
                AddNote(panel, "须由君主授予", width);
            }
            AddEffects(panel, title.Influences.Values, width);
            AddConditions(panel, "获得条件", Unchecked(title.Conditions.Concat(title.ArchitectureConditions).Concat(title.FactionConditions)), width);
            if (title.LoseConditions.Count > 0)
            {
                AddConditions(panel, "失去条件", Unchecked(title.LoseConditions), width);
            }
            AddHolders(panel, title.Name, AvailablePersons.Where(x => x.RealTitles.Contains(title)), width);
        }

        #endregion

        #region 技能

        public static void ShowSkills()
        {
            var skills = Session.Current.Scenario.GameCommonData.AllSkills.Values.ToList();
            var columns = new[]
            {
                new InfoColumn<Skill>("名称", 160, (x, w) => CreateNameCell(x.Name, MyraSkin.TextPrimary, x.Influences.Values), x => x.Name),
                new InfoColumn<Skill>("等级", 50, x => x.Level.ToString(), x => x.Level),
                new InfoColumn<Skill>("效果", 0, (x, w) => CreateEffectChips(x.Influences.Values, w), x => x.InfluenceCount)
            };
            var filters = new[]
            {
                new InfoFilter<Skill>("战斗", x => x.Combat),
                new InfoFilter<Skill>("非战斗", x => !x.Combat)
            };
            InfoListDialog<Skill>.ShowWithDetail("技能", skills, columns, filters, FillSkill);
        }

        private static void FillSkill(VerticalStackPanel panel, Skill skill, int width)
        {
            var notes = new List<string> { "技能" };
            if (skill.Combat)
            {
                notes.Add("战斗");
            }
            string ability = GetAbilityName(skill.RelatedAbility);
            if (ability != null)
            {
                notes.Add("相关" + ability);
            }
            AddDetailTop(panel, string.Join(" · ", notes), skill.Name, MyraSkin.TextPrimary, width);
            AddStat(panel, "等级", skill.Level.ToString(), width);
            AddEffects(panel, skill.Influences.Values, width);
            AddConditions(panel, "修习条件", Unchecked(skill.Conditions), width);
            AddHolders(panel, skill.Name, AvailablePersons.Where(x => x.Skills.ContainsKey(skill.ID)), width);
        }

        #endregion

        #region 特技

        public static void ShowStunts()
        {
            var stunts = Session.Current.Scenario.GameCommonData.AllStunts.Values.ToList();
            var columns = new[]
            {
                new InfoColumn<Stunt>("名称", 130, (x, w) => CreateNameCell(x.Name, MyraSkin.TextPrimary, x.Influences.Values), x => x.Name),
                new InfoColumn<Stunt>("战意", 50, x => x.Combativity.ToString(), x => x.Combativity),
                new InfoColumn<Stunt>("持续", 60, x => GetStuntDays(x) + "天", x => GetStuntDays(x)),
                new InfoColumn<Stunt>("效果", 0, (x, w) => CreateEffectChips(x.Influences.Values, w), x => x.Influences.Count)
            };
            InfoListDialog<Stunt>.ShowWithDetail("特技", stunts, columns, null, FillStunt);
        }

        private static int GetStuntDays(Stunt stunt)
        {
            return stunt.Period * Session.Parameters.DayInTurn;
        }

        private static void FillStunt(VerticalStackPanel panel, Stunt stunt, int width)
        {
            AddDetailTop(panel, "战斗特技", stunt.Name, MyraSkin.TextPrimary, width);
            AddStat(panel, "战意消耗", stunt.Combativity.ToString(), width);
            AddStat(panel, "持续", GetStuntDays(stunt) + " 天", width);
            AddEffects(panel, stunt.Influences.Values, width);
            AddConditions(panel, "使用条件", Unchecked(stunt.CastConditions), width);
            AddConditions(panel, "修习条件", Unchecked(stunt.LearnConditions), width);
            AddHolders(panel, stunt.Name, AvailablePersons.Where(x => x.Stunts.ContainsKey(stunt.ID)), width);
        }

        #endregion

        #region 兵种

        /// <summary>
        /// 不传数据时显示全部兵种；势力右键菜单传入势力可用的兵种和标题
        /// </summary>
        public static void ShowMilitaryKinds(string title = "兵种", IEnumerable<MilitaryKind> items = null)
        {
            var kinds = (items ?? Session.Current.Scenario.GameCommonData.AllMilitaryKinds.Values).Where(x => !x.IsShell).ToList();
            var columns = new[]
            {
                new InfoColumn<MilitaryKind>("名称", MilitaryKindNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<MilitaryKind>("种类", 56, x => x.TypeName, x => (int)x.Type),
                new InfoColumn<MilitaryKind>("攻击", 56, x => x.Offence.ToString(), x => x.Offence),
                new InfoColumn<MilitaryKind>("防御", 56, x => x.Defence.ToString(), x => x.Defence),
                new InfoColumn<MilitaryKind>("行动", 56, x => x.Movability.ToString(), x => x.Movability),
                new InfoColumn<MilitaryKind>("射程", 56, x => x.OffenceRadius.ToString(), x => x.OffenceRadius),
                new InfoColumn<MilitaryKind>("费用", 64, x => x.CreateCost.ToString(), x => x.CreateCost)
            };
            var filters = kinds.Select(x => x.Type).Distinct().OrderBy(x => (int)x)
                .Select(type => new InfoFilter<MilitaryKind>(type.GetDescription(), x => x.Type == type));
            InfoListDialog<MilitaryKind>.ShowWithDetail(title, kinds, columns, filters,
                (panel, kind, width, list) => FillMilitaryKind(panel, kind, width, list.Select), CreateMilitaryKindViews());
        }

        // 上次看的是兵种详情的哪个页签，换兵种时停在同一页
        private static int militaryKindTab;

        // 兵种详情：顶部名称和说明，五个主要数值的卡片，下面分战斗、编成、地形、升级四个页签
        private static void FillMilitaryKind(VerticalStackPanel panel, MilitaryKind kind, int width, Action<MilitaryKind> onSelect)
        {
            AddDetailTop(panel, "兵种 · " + kind.TypeName, kind.Name, MyraSkin.TextPrimary, width);
            string note = kind.Description?.Trim() ?? "";
            if (kind.CanLevelUp && kind.LevelUpKindID.Count > 0)
            {
                note += (note.Length > 0 ? " " : "") + "[" + kind.LevelUpExperience + " 经验升级]";
            }
            if (note.Length > 0)
            {
                AddNote(panel, note, width);
            }
            panel.Widgets.Add(CreateSpacer(6));
            panel.Widgets.Add(CreateKindCards(kind, width));
            panel.Widgets.Add(CreateSpacer(8));
            var pages = new List<(string, Widget)>
            {
                ("战斗", BuildKindCombat(kind, width)),
                ("编成", BuildKindCreation(kind, width)),
                ("地形", BuildKindTerrain(kind, width)),
                ("升级", BuildKindLevelUp(kind, width, onSelect, null))
            };
            AddTabs(panel, pages, width, militaryKindTab, x => militaryKindTab = x);
        }

        // 攻击、防御、行动、射程、费用各一张卡片，费用用金色
        private static Widget CreateKindCards(MilitaryKind kind, int width)
        {
            return CreateCards(new List<(string, string, Color)>
            {
                ("攻击", kind.Offence.ToString(), MyraSkin.TextPrimary),
                ("防御", kind.Defence.ToString(), MyraSkin.TextPrimary),
                ("行动", kind.Movability.ToString(), MyraSkin.TextPrimary),
                ("射程", kind.OffenceRadius.ToString(), MyraSkin.TextPrimary),
                ("费用", kind.CreateCost.ToString(), Accent)
            }, width);
        }

        private static Widget BuildKindCombat(MilitaryKind kind, int width)
        {
            var page = CreatePage();
            page.Widgets.Add(CreateStatGrid(new List<(string, string)>
            {
                ("攻击 / 防御", kind.Offence + " / " + kind.Defence),
                ("每单位攻防", kind.OffencePerScale + " / " + kind.DefencePerScale),
                ("每百经验攻防", kind.OffencePer100Experience + " / " + kind.DefencePer100Experience),
                ("行动力", kind.Movability.ToString()),
                ("攻击范围", kind.OffenceRadius.ToString()),
                ("计略范围", kind.StratagemRadius.ToString()),
                ("视野范围", kind.ViewRadius.ToString()),
                ("受火伤率", FormatRate(kind.FireDamageRate)),
                ("建筑伤害率", FormatRate(kind.ArchitectureDamageRate)),
                ("建筑反击率", FormatRate(kind.ArchitectureCounterDamageRate))
            }, width));

            AddKindTraits(page, kind, width);
            return page;
        }

        // 兵种的攻击特性（标签）和兵种特性（效果），兵种和编队的战斗页共用
        private static void AddKindTraits(VerticalStackPanel page, MilitaryKind kind, int width)
        {
            var traits = new List<string>();
            AddTrait(traits, kind.ObliqueOffence, "斜向攻击");
            AddTrait(traits, kind.ArrowOffence, "箭矢攻击");
            AddTrait(traits, kind.AirOffence, "远程攻击");
            AddTrait(traits, kind.ContactOffence, "接触攻击");
            AddTrait(traits, kind.CounterOffence, "可反击");
            AddTrait(traits, !kind.BeCountered, "不被反击");
            AddTrait(traits, kind.OffenceOnlyBeforeMove, "移动前攻击");
            AddTrait(traits, kind.ObliqueStratagem, "斜向计略");
            AddTrait(traits, kind.ObliqueView, "斜向视野");
            if (traits.Count > 0)
            {
                page.Widgets.Add(CreateDetailHeader("攻击特性", width));
                AddFlow(page, traits.Select(x => CreateTag(x, Accent)), width);
            }
            if (kind.Influences.Count > 0)
            {
                page.Widgets.Add(CreateDetailHeader("兵种特性", width));
                AddEffects(page, kind.Influences, width);
            }
        }

        private static Widget BuildKindCreation(MilitaryKind kind, int width)
        {
            var page = CreatePage();
            page.Widgets.Add(CreateStatList(new List<(string, string)>
            {
                ("规模", kind.MinScale + " ～ " + kind.MaxScale),
                ("新编费用", kind.CreateCost.ToString()),
                ("新编技术", kind.CreateTechnology.ToString()),
                ("每兵所需技巧", kind.PointsPerSoldier.ToString()),
                ("统率要求", kind.MinCommand.ToString()),
                ("编队上限", kind.RecruitLimit.ToString()),
                ("每兵耗粮", kind.FoodPerSoldier.ToString()),
                ("新编地点", kind.CreateBesideWater ? "须在水边" : "不限")
            }, width));
            AddConditions(page, "编成条件", Unchecked(kind.CreateConditions), width);
            return page;
        }

        private static readonly string[] TerrainNames = { "平原", "草原", "森林", "湿地", "山地", "水域", "峻岭", "荒地", "栈道", "瘴气林" };

        // 地形卡片的配色：优势金色，劣势橙红
        private static readonly Color TerrainGoodFill = new Color(0x3A, 0x32, 0x1E);
        private static readonly Color TerrainBadFill = new Color(0x34, 0x22, 0x1E);
        private static readonly Color TerrainBlockedFill = new Color(0x1E, 0x18, 0x18);

        // 地形按乘数分成优势、适应、劣势三组，每组一排卡片：地形名、乘数、适性。
        // 适性超过行动力（一格都走不了）的用虚线框，排在劣势组最后；空的组不显示
        private static Widget BuildKindTerrain(MilitaryKind kind, int width)
        {
            int[] adaptabilities =
            {
                kind.PlainAdaptability, kind.GrasslandAdaptability, kind.ForrestAdaptability, kind.MarshAdaptability, kind.MountainAdaptability,
                kind.WaterAdaptability, kind.RidgeAdaptability, kind.WastelandAdaptability, kind.DesertAdaptability, kind.CliffAdaptability
            };
            float[] rates =
            {
                kind.PlainRate, kind.GrasslandRate, kind.ForrestRate, kind.MarshRate, kind.MountainRate,
                kind.WaterRate, kind.RidgeRate, kind.WastelandRate, kind.DesertRate, kind.CliffRate
            };
            var terrains = Enumerable.Range(0, TerrainNames.Length)
                .Select(i => (Name: TerrainNames[i], Adaptability: adaptabilities[i], Rate: rates[i], Blocked: adaptabilities[i] > kind.Movability))
                .ToList();

            var page = CreatePage();
            var groups = new[]
            {
                ("优势", "乘数 > 1", Accent, terrains.Where(x => !x.Blocked && x.Rate > 1.001f).OrderByDescending(x => x.Rate).ThenBy(x => x.Adaptability)),
                ("适应", "乘数 = 1", MyraSkin.TextPrimary, terrains.Where(x => !x.Blocked && Math.Abs(x.Rate - 1) <= 0.001f).OrderBy(x => x.Adaptability)),
                ("劣势", "乘数 < 1", MyraSkin.TextHostile, terrains.Where(x => x.Blocked || x.Rate < 0.999f).OrderBy(x => x.Blocked).ThenByDescending(x => x.Rate).ThenBy(x => x.Adaptability))
            };
            foreach (var (title, note, color, items) in groups)
            {
                var list = items.ToList();
                if (list.Count == 0)
                {
                    continue;
                }
                var header = new HorizontalStackPanel
                {
                    Spacing = 8,
                    Height = TerrainGroupHeaderHeight
                };
                var titleLabel = CreateLabel(title, 15, color);
                titleLabel.VerticalAlignment = VerticalAlignment.Center;
                header.Widgets.Add(titleLabel);
                var noteLabel = CreateLabel(note, 13, MyraSkin.TextSecondary);
                noteLabel.VerticalAlignment = VerticalAlignment.Center;
                header.Widgets.Add(noteLabel);
                page.Widgets.Add(header);
                page.Widgets.Add(CreateTerrainCards(list, width));
                page.Widgets.Add(CreateSpacer(6));
            }
            AddNote(page, "乘数影响战力；适性越小越好走。", width);
            return page;
        }

        // 每行四张卡片
        private static Widget CreateTerrainCards(List<(string Name, int Adaptability, float Rate, bool Blocked)> terrains, int width)
        {
            int cardWidth = (width - (TerrainCardsPerRow - 1) * TerrainCardSpacing) / TerrainCardsPerRow;
            var rows = new VerticalStackPanel
            {
                Spacing = TerrainCardSpacing
            };
            HorizontalStackPanel row = null;
            foreach (var terrain in terrains)
            {
                if (row == null || row.Widgets.Count >= TerrainCardsPerRow)
                {
                    row = new HorizontalStackPanel
                    {
                        Spacing = TerrainCardSpacing
                    };
                    rows.Widgets.Add(row);
                }
                row.Widgets.Add(CreateTerrainCard(terrain, cardWidth));
            }
            return rows;
        }

        private static Widget CreateTerrainCard((string Name, int Adaptability, float Rate, bool Blocked) terrain, int width)
        {
            Color rateColor;
            IBrush background;
            if (terrain.Blocked)
            {
                rateColor = MyraSkin.TextHostile;
                background = new FrameBrush(TerrainBlockedFill, MyraSkin.TextHostile * 0.45f, true);
            }
            else if (terrain.Rate > 1.001f)
            {
                rateColor = Accent;
                background = new FrameBrush(TerrainGoodFill, GoldDim, false);
            }
            else if (terrain.Rate < 0.999f)
            {
                rateColor = MyraSkin.TextHostile;
                background = new FrameBrush(TerrainBadFill, MyraSkin.TextHostile * 0.35f, false);
            }
            else
            {
                rateColor = MyraSkin.TextPrimary;
                background = new FrameBrush(ChipColor, LineColor, false);
            }
            var content = new VerticalStackPanel
            {
                Spacing = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var name = CreateLabel(terrain.Name, 14, MyraSkin.TextPrimary);
            name.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(name);
            var rate = CreateLabel("×" + FormatRate(terrain.Rate), 22, rateColor);
            rate.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(rate);
            var adaptability = CreateLabel("适性 " + terrain.Adaptability, 12, MyraSkin.TextSecondary);
            adaptability.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(adaptability);
            var card = new Panel
            {
                Width = width,
                Height = TerrainCardHeight,
                Background = background
            };
            card.Widgets.Add(content);
            return card;
        }

        // 升级路线：左边是由哪些兵种升来，中间是本兵种，右边是能升成的兵种和攻防的增减，节点之间画连线。
        // 没有来源的不画左列，本兵种靠左；不能再升级的不画右列。给了onSelect时点节点跳到那个兵种。
        // experience不为null时（编队的详情）底部写编队现有的经验和升级所需
        private static Widget BuildKindLevelUp(MilitaryKind kind, int width, Action<MilitaryKind> onSelect, int? experience)
        {
            var page = CreatePage();
            var all = Session.Current.Scenario.GameCommonData.AllMilitaryKinds;
            var targets = kind.CanLevelUp ? kind.LevelUpKindID.Select(x => all.GetValueOrDefault(x)).Where(x => x != null).ToList() : new List<MilitaryKind>();
            var sources = all.Values.Where(x => x.CanLevelUp && x.LevelUpKindID.Contains(kind.ID)).ToList();
            if (targets.Count == 0 && sources.Count == 0)
            {
                AddNote(page, "此兵种不能升级，也不由其他兵种升级而来。", width);
                return page;
            }
            page.Widgets.Add(CreateLabel(onSelect != null ? "升级路线 · 点击节点查看该兵种" : "升级路线", 13, MyraSkin.TextSecondary));
            page.Widgets.Add(CreateSpacer(6));

            // 节点宽度总按三列算，少一列时右边空着，节点大小不随兵种变
            int nodeWidth = (width - 2 * RouteGap) / 3;
            int height = Math.Max(RouteNodeHeight, Math.Max(GetColumnHeight(sources.Count), GetColumnHeight(targets.Count)));
            int middleX = sources.Count > 0 ? nodeWidth + RouteGap : 0;
            int rightX = middleX + nodeWidth + RouteGap;
            int centerY = height / 2;
            var canvas = new Panel
            {
                Width = width,
                Height = height
            };

            var sourceCenters = new List<int>();
            for (int i = 0; i < sources.Count; i++)
            {
                MilitaryKind source = sources[i];
                int top = GetNodeTop(height, sources.Count, i);
                sourceCenters.Add(top + RouteNodeHeight / 2);
                Place(canvas, CreateRouteNode(source.Name, "攻" + source.Offence + " 防" + source.Defence, nodeWidth, false, onSelect == null ? null : () => onSelect(source)), 0, top);
            }
            Place(canvas, CreateRouteNode(kind.Name, "攻" + kind.Offence + " 防" + kind.Defence, nodeWidth, true, null), middleX, centerY - RouteNodeHeight / 2);
            var targetCenters = new List<int>();
            for (int i = 0; i < targets.Count; i++)
            {
                MilitaryKind target = targets[i];
                int top = GetNodeTop(height, targets.Count, i);
                targetCenters.Add(top + RouteNodeHeight / 2);
                string note = "攻" + FormatDelta(target.Offence - kind.Offence) + " 防" + FormatDelta(target.Defence - kind.Defence);
                Place(canvas, CreateRouteNode(target.Name, note, nodeWidth, false, onSelect == null ? null : () => onSelect(target)), rightX, top);
            }
            // 连线：一边的节点各自横接到两列中间的竖线，竖线再横接到本兵种
            if (sources.Count > 0)
            {
                AddConnectors(canvas, nodeWidth, middleX, sourceCenters, centerY, true);
            }
            if (targets.Count > 0)
            {
                AddConnectors(canvas, middleX + nodeWidth, rightX, targetCenters, centerY, false);
            }
            page.Widgets.Add(canvas);

            if (targets.Count > 0)
            {
                page.Widgets.Add(CreateSpacer(10));
                var card = new Panel
                {
                    Height = 44,
                    Padding = new Thickness(16, 0, 16, 0),
                    Background = new SolidBrush(ChipColor)
                };
                var name = CreateLabel(experience.HasValue ? "经验 / 升级所需" : "升级所需经验", 15, MyraSkin.TextPrimary);
                name.VerticalAlignment = VerticalAlignment.Center;
                card.Widgets.Add(name);
                string text = experience.HasValue ? experience.Value + " / " + kind.LevelUpExperience : kind.LevelUpExperience.ToString();
                var value = CreateLabel(text, 16, experience >= kind.LevelUpExperience ? MyraSkin.TextFriendly : Accent);
                value.HorizontalAlignment = HorizontalAlignment.Right;
                value.VerticalAlignment = VerticalAlignment.Center;
                card.Widgets.Add(value);
                page.Widgets.Add(card);
            }
            return page;
        }

        // 路线上的一个节点：本兵种金框金字；其他兵种有onClick时可以点，没有时只显示
        private static Widget CreateRouteNode(string name, string note, int width, bool current, Action onClick)
        {
            var content = new VerticalStackPanel
            {
                Spacing = 2,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var nameLabel = CreateLabel(name, 15, current ? Accent : MyraSkin.TextPrimary);
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(nameLabel);
            var noteLabel = CreateLabel(note, 12, MyraSkin.TextSecondary);
            noteLabel.HorizontalAlignment = HorizontalAlignment.Center;
            content.Widgets.Add(noteLabel);
            if (current || onClick == null)
            {
                var box = new Panel
                {
                    Width = width,
                    Height = RouteNodeHeight,
                    Background = current ? new FrameBrush(new Color(36, 52, 60), GoldDim, false) : new FrameBrush(ChipColor, LineColor, false)
                };
                box.Widgets.Add(content);
                return box;
            }
            Button button = CreateFlatButton(content, width, RouteNodeHeight, new FrameBrush(ChipColor, LineColor, false));
            button.Click += (sender, args) => onClick();
            return button;
        }

        private static int GetColumnHeight(int count)
        {
            return count <= 0 ? 0 : count * RouteNodeHeight + (count - 1) * RouteNodeSpacing;
        }

        // 一列节点在画布里上下居中
        private static int GetNodeTop(int height, int count, int index)
        {
            return (height - GetColumnHeight(count)) / 2 + index * (RouteNodeHeight + RouteNodeSpacing);
        }

        // 两列之间的连线：左列节点右边缘（fromX）到右列节点左边缘（toX）之间立一条竖线。
        // 那一列的各节点（中心高度centers）横接到竖线，竖线另一边横接到本兵种（中心高度centerY）
        private static void AddConnectors(Panel canvas, int fromX, int toX, List<int> centers, int centerY, bool centersOnLeft)
        {
            int trunkX = (fromX + toX) / 2;
            foreach (int y in centers)
            {
                if (centersOnLeft)
                {
                    AddLine(canvas, fromX, y, trunkX - fromX, 1);
                }
                else
                {
                    AddLine(canvas, trunkX, y, toX - trunkX, 1);
                }
            }
            if (centersOnLeft)
            {
                AddLine(canvas, trunkX, centerY, toX - trunkX, 1);
            }
            else
            {
                AddLine(canvas, fromX, centerY, trunkX - fromX, 1);
            }
            int top = Math.Min(centers.Min(), centerY);
            int bottom = Math.Max(centers.Max(), centerY);
            if (bottom > top)
            {
                AddLine(canvas, trunkX, top, 1, bottom - top + 1);
            }
        }

        private static void AddLine(Panel canvas, int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                return;
            }
            Place(canvas, new Panel
            {
                Width = width,
                Height = height,
                Background = new SolidBrush(LineColor)
            }, x, y);
        }

        // 画布里按左上角坐标摆放
        private static void Place(Panel canvas, Widget widget, int x, int y)
        {
            widget.HorizontalAlignment = HorizontalAlignment.Left;
            widget.VerticalAlignment = VerticalAlignment.Top;
            widget.Left = x;
            widget.Top = y;
            canvas.Widgets.Add(widget);
        }

        private static string FormatDelta(int delta)
        {
            return delta > 0 ? "+" + delta : delta < 0 ? delta.ToString() : "±0";
        }

        private static void AddTrait(List<string> traits, bool has, string name)
        {
            if (has)
            {
                traits.Add(name);
            }
        }

        private static string FormatRate(float rate)
        {
            return rate.ToString("0.##");
        }

        #endregion

        #region 宝物

        /// <summary>
        /// 不传数据时显示全部宝物；城池、势力右键菜单传入相关宝物和标题
        /// </summary>
        public static void ShowTreasures(string title = "宝物", IEnumerable<Treasure> items = null)
        {
            var treasures = (items ?? Session.Current.Scenario.Treasures.Values).ToList();
            var columns = new[]
            {
                new InfoColumn<Treasure>("名称", TreasureNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Treasure>("价值", 56, x => x.Worth.ToString(), x => x.Worth),
                new InfoColumn<Treasure>("效果", 0, (x, w) => CreateEffectChips(x.Influences.Values, w), x => x.Influences.Count),
                new InfoColumn<Treasure>("所属", 80, x => x.BelongedPersonString, x => x.BelongedPersonString)
            };
            IEnumerable<InfoFilter<Treasure>> filters = new[]
            {
                new InfoFilter<Treasure>("有主", x => x.BelongedPerson != null),
                new InfoFilter<Treasure>("隐藏", x => x.BelongedPerson == null && x.HidePlace != null),
                new InfoFilter<Treasure>("下落不明", x => x.BelongedPerson == null && x.HidePlace == null)
            };
            if (items != null)
            {
                filters = KeepUsefulFilters(filters, treasures);
            }
            InfoListDialog<Treasure>.ShowWithDetail(title, treasures, columns, filters, FillTreasure,
                CreateTreasureViews());
        }

        private static void FillTreasure(VerticalStackPanel panel, Treasure treasure, int width)
        {
            panel.Widgets.Add(CreateLabel("宝物", 13, MyraSkin.TextSecondary));
            var header = new HorizontalStackPanel
            {
                Spacing = 14
            };
            header.Widgets.Add(new Image
            {
                Renderable = new ArchitectureDetailDialog.TextureImage(PersonDetailDialog.GetTreasurePicture(treasure), TreasurePictureSize, TreasurePictureSize),
                Width = TreasurePictureSize,
                Height = TreasurePictureSize,
                Border = new SolidBrush(GoldDim),
                BorderThickness = new Thickness(1)
            });
            var names = new VerticalStackPanel
            {
                Spacing = 6,
                VerticalAlignment = VerticalAlignment.Center
            };
            names.Widgets.Add(CreateWrappedLabel(treasure.Name, 26, MyraSkin.TextPrimary, width - TreasurePictureSize - 14));
            names.Widgets.Add(CreateLabel("价值 " + treasure.Worth, 14, Accent));
            header.Widgets.Add(names);
            panel.Widgets.Add(header);
            if (!string.IsNullOrWhiteSpace(treasure.Description))
            {
                AddNote(panel, treasure.Description, width);
            }

            panel.Widgets.Add(CreateDetailHeader("下落", width));
            if (treasure.BelongedPerson != null)
            {
                Person owner = treasure.BelongedPerson;
                var row = new HorizontalStackPanel
                {
                    Height = LinkHeight
                };
                row.Widgets.Add(CreateFixedLabel("所属", MyraSkin.TextSecondary, StatLabelWidth, TextHorizontalAlignment.Left));
                row.Widgets.Add(CreateLink(owner.Name, () => PersonDetailDialog.Show(owner, false)));
                panel.Widgets.Add(row);
            }
            else
            {
                AddStat(panel, "所属", "无", width);
            }
            AddStat(panel, "隐藏于", treasure.HidePlaceString, width);
            if (treasure.AppearYear > 0)
            {
                AddStat(panel, "出现年份", treasure.AppearYear + "年", width);
            }
            if (treasure.Durability > 0)
            {
                AddStat(panel, "耐久", treasure.Durability.ToString(), width);
            }
            AddEffects(panel, treasure.Influences.Values, width);
        }

        #endregion

        #region 官爵

        public static void ShowGuanjues()
        {
            var guanjues = Session.Current.Scenario.GameCommonData.AllOfficialTitleKinds.Values.OrderBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<OfficialTitleKind>("名称", 90, x => x.Name, x => x.ID, x => MyraSkin.TextTitle),
                new InfoColumn<OfficialTitleKind>("声望上限", 90, x => x.ReputationCap.ToString(), x => x.ReputationCap),
                new InfoColumn<OfficialTitleKind>("需要贡献", 90, x => x.RequiredContribution.ToString(), x => x.RequiredContribution),
                new InfoColumn<OfficialTitleKind>("需要城池", 80, x => x.RequiredArchitecture.ToString(), x => x.RequiredArchitecture),
                new InfoColumn<OfficialTitleKind>("忠诚", 60, x => FormatDelta(x.Loyalty), x => x.Loyalty)
            };
            InfoListDialog<OfficialTitleKind>.ShowWithDetail("官爵", guanjues, columns, null, (panel, guanjue, width) => FillGuanjue(panel, guanjue, guanjues.IndexOf(guanjue) + 1, width));
        }

        private static void FillGuanjue(VerticalStackPanel panel, OfficialTitleKind guanjue, int rank, int width)
        {
            AddDetailTop(panel, "官爵 · 第" + rank + "等", guanjue.Name, MyraSkin.TextTitle, width);
            panel.Widgets.Add(CreateSpacer(6));
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("声望上限", guanjue.ReputationCap.ToString(), MyraSkin.TextPrimary),
                ("需要贡献", guanjue.RequiredContribution.ToString(), Accent),
                ("需要城池", guanjue.RequiredArchitecture.ToString(), Accent)
            }, width));
            panel.Widgets.Add(CreateSpacer(8));
            panel.Widgets.Add(CreateStatList(new List<(string, string)>
            {
                ("封官所需朝廷贡献度", guanjue.RequiredContribution.ToString()),
                ("封官所需城池数", guanjue.RequiredArchitecture.ToString()),
                ("势力声望上限", guanjue.ReputationCap.ToString()),
                ("部下忠诚变化", FormatDelta(guanjue.Loyalty)),
                ("全图公告封官", guanjue.ShowDialog ? "是" : "否")
            }, width));

            var factions = Session.Current.Scenario.Factions.Values.Where(x => x.guanjue == guanjue.ID).OrderBy(x => x.ID).ToList();
            panel.Widgets.Add(CreateDetailHeader("受封势力 · " + factions.Count + "家", width));
            if (factions.Count == 0)
            {
                panel.Widgets.Add(CreateLabel("无", 14, MyraSkin.TextSecondary));
            }
            else
            {
                AddFlow(panel, factions.Select(x => CreateTag(x.Name, Accent)), width);
            }
        }

        #endregion

        #region 设施

        public static void ShowFacilities()
        {
            var levels = Session.Current.Scenario.GameCommonData.AllFacilityKindLevels.Values.OrderBy(x => x.KindId).ThenBy(x => x.Level).ToList();
            var kinds = Session.Current.Scenario.GameCommonData.AllFacilityKinds;
            FacilityType GetFacilityType(FacilityKindLevel level) => kinds.TryGetValue(level.KindId, out FacilityKind kind) ? kind.Type : FacilityType.General;
            var columns = new[]
            {
                new InfoColumn<FacilityKindLevel>("名称", 130, GetFacilityName, x => x.KindId * 100 + x.Level),
                new InfoColumn<FacilityKindLevel>("类别", 50, x => ArchitectureDetailDialog.GetTypeName(GetFacilityType(x)), x => (int)GetFacilityType(x)),
                new InfoColumn<FacilityKindLevel>("空间", 50, x => x.PositionOccupied.ToString(), x => x.PositionOccupied),
                new InfoColumn<FacilityKindLevel>("建造费", 64, x => x.FundCost.ToString(), x => x.FundCost),
                new InfoColumn<FacilityKindLevel>("维持", 56, x => (x.MaintenanceCost * 30).ToString(), x => x.MaintenanceCost),
                new InfoColumn<FacilityKindLevel>("效果", 0, (x, w) => CreateEffectChips(x.Influences, w), x => x.Influences.Count)
            };
            var filters = new[] { FacilityType.General, FacilityType.Special, FacilityType.Military }
                .Select(type => new InfoFilter<FacilityKindLevel>(ArchitectureDetailDialog.GetTypeName(type), x => GetFacilityType(x) == type));
            InfoListDialog<FacilityKindLevel>.ShowWithDetail("设施", levels, columns, filters, (panel, level, width) => FillFacility(panel, level, GetFacilityType(level), width));
        }

        private static string GetFacilityName(FacilityKindLevel level)
        {
            return level.Level > 1 ? level.Name + " " + level.Level + "级" : level.Name;
        }

        // 每城、每势力的数量上限，9999表示不限
        private static string FormatLimit(int limit)
        {
            return limit >= 9999 ? "不限" : limit.ToString();
        }

        private static void FillFacility(VerticalStackPanel panel, FacilityKindLevel level, FacilityType type, int width)
        {
            // 图片放左边，类别和名称放右边，没有图片时只有文字
            var header = new HorizontalStackPanel
            {
                Spacing = 12
            };
            string picture = ArchitectureDetailDialog.FacilityPicturePath + level.KindId + ".png";
            int textWidth = width;
            if (Platform.Current.FileExists(picture))
            {
                header.Widgets.Add(new Image
                {
                    Renderable = new ArchitectureDetailDialog.TextureImage(picture, FacilityPictureWidth, FacilityPictureHeight, premultiply: true),
                    Width = FacilityPictureWidth,
                    Height = FacilityPictureHeight,
                    VerticalAlignment = VerticalAlignment.Center,
                    Border = new SolidBrush(GoldDim),
                    BorderThickness = new Thickness(1)
                });
                textWidth = width - FacilityPictureWidth - 12;
            }
            var names = new VerticalStackPanel
            {
                Spacing = 4,
                VerticalAlignment = VerticalAlignment.Center
            };
            names.Widgets.Add(CreateLabel("设施 · " + ArchitectureDetailDialog.GetTypeName(type), 13, MyraSkin.TextSecondary));
            names.Widgets.Add(CreateWrappedLabel(GetFacilityName(level), 24, MyraSkin.TextPrimary, textWidth));
            header.Widgets.Add(names);
            panel.Widgets.Add(header);
            panel.Widgets.Add(CreateSpacer(8));
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("空间", level.PositionOccupied.ToString(), MyraSkin.TextPrimary),
                ("技术", level.TechnologyNeeded.ToString(), MyraSkin.TextPrimary),
                ("技巧", level.PointCost.ToString(), MyraSkin.TextPrimary),
                ("工期", level.DaysText + "天", MyraSkin.TextPrimary),
                ("资金", level.FundCost.ToString(), Accent)
            }, width));
            panel.Widgets.Add(CreateSpacer(8));
            var stats = new List<(string, string)>
            {
                ("维持费", level.MaintenanceCost * 30 + " / 月"),
                ("耐久", level.Endurance.ToString()),
                // IsDemolishable名字起反了，数据里为true表示不可拆除
                ("可拆除", level.IsDemolishable ? "否" : "是"),
                ("每城上限", FormatLimit(level.ArchitectureLimit)),
                ("每势力上限", FormatLimit(level.FactionLimit))
            };
            if (level.ConcubineCapacity > 0)
            {
                stats.Add(("可容纳美女", level.ConcubineCapacity + " 名"));
            }
            panel.Widgets.Add(CreateStatList(stats, width));
            AddEffects(panel, level.Influences, width);
            AddConditions(panel, "建造条件", Unchecked(level.Conditions), width);
        }

        #endregion

        #region 地形

        public static void ShowTerrainDetails()
        {
            var terrains = Session.Current.Scenario.GameCommonData.AllTerrainDetails.Values.OrderBy(x => x.ID).ToList();
            var columns = new[]
            {
                new InfoColumn<TerrainDetail>("名称", 90, x => x.Name, x => x.ID),
                new InfoColumn<TerrainDetail>("粮道建设", 80, x => x.RoutewayBuildFundCost.ToString(), x => x.RoutewayBuildFundCost),
                new InfoColumn<TerrainDetail>("粮道维持", 80, x => x.RoutewayActiveFundCost.ToString(), x => x.RoutewayActiveFundCost),
                new InfoColumn<TerrainDetail>("粮草蕴藏", 80, x => x.FoodDeposit.ToString(), x => x.FoodDeposit),
                new InfoColumn<TerrainDetail>("受火伤", 64, x => x.FireDamageRateString, x => x.FireDamageRate),
                new InfoColumn<TerrainDetail>("通行", 50, x => x.TroopPassable ? "可" : "不可", x => x.TroopPassable ? 1 : 0,
                    x => x.TroopPassable ? MyraSkin.TextPrimary : MyraSkin.TextDisabled)
            };
            InfoListDialog<TerrainDetail>.ShowWithDetail("地形", terrains, columns, null, FillTerrain);
        }

        private static void FillTerrain(VerticalStackPanel panel, TerrainDetail terrain, int width)
        {
            AddDetailTop(panel, "地形", terrain.Name, MyraSkin.TextPrimary, width);
            if (!terrain.TroopPassable)
            {
                AddNote(panel, "所有兵种都无法进入。", width);
            }
            panel.Widgets.Add(CreateSpacer(6));
            // 四季的粮草率，冬天往往最低
            panel.Widgets.Add(CreateCards(new List<(string, string, Color)>
            {
                ("春季粮草", terrain.FoodSpringRateString, MyraSkin.TextPrimary),
                ("夏季粮草", terrain.FoodSummerRateString, MyraSkin.TextPrimary),
                ("秋季粮草", terrain.FoodAutumnRateString, Accent),
                ("冬季粮草", terrain.FoodWinterRateString, MyraSkin.TextPrimary)
            }, width));

            panel.Widgets.Add(CreateDetailHeader("粮道", width));
            panel.Widgets.Add(CreateStatGrid(new List<(string, string)>
            {
                ("建设资金", terrain.RoutewayBuildFundCost.ToString()),
                ("维持资金", terrain.RoutewayActiveFundCost.ToString()),
                ("建设工作量", terrain.RoutewayBuildWorkCost.ToString()),
                ("运粮消耗率", FormatRate(terrain.RoutewayConsumptionRate))
            }, width));

            panel.Widgets.Add(CreateDetailHeader("粮草", width));
            panel.Widgets.Add(CreateStatGrid(new List<(string, string)>
            {
                ("蕴藏量", terrain.FoodDeposit.ToString()),
                ("复原天数", terrain.FoodRegainDays + " 天")
            }, width));

            panel.Widgets.Add(CreateDetailHeader("其他", width));
            panel.Widgets.Add(CreateStatGrid(new List<(string, string)>
            {
                ("受火伤率", terrain.FireDamageRateString),
                ("视野可穿透", terrain.ViewThrough ? "是" : "否"),
                ("城池可扩建到此", terrain.CanExtendInto ? "是" : "否"),
                ("部队可通行", terrain.TroopPassable ? "是" : "否")
            }, width));
        }

        #endregion

        #region 列表单元格

        // 名称，后面跟限定生效时机的小标签（主将、君主等）。只有第一条就是限定、所有效果都受限时才挂标签，
        // 部分受限的在效果标签上用虚线框表示
        private static Widget CreateNameCell(string name, Color color, IEnumerable<Influence> influences)
        {
            var cell = new HorizontalStackPanel
            {
                Spacing = 8
            };
            var nameLabel = CreateLabel(name, 17, color);
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            cell.Widgets.Add(nameLabel);
            foreach (string tag in influences.TakeWhile(IsRestriction).Select(x => RestrictionTags.GetValueOrDefault(x.Kind.ID)).Where(x => x != null).Distinct())
            {
                var holder = new Panel
                {
                    Padding = new Thickness(5, 1, 5, 1),
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = new FrameBrush(ChipColor, LineColor, false)
                };
                holder.Widgets.Add(CreateLabel(tag, 12, MyraSkin.TextSecondary));
                cell.Widgets.Add(holder);
            }
            return cell;
        }

        // 效果标签：名称灰白、数值金色，一行放不下的在末尾写“+N”。
        // 名称后面没挂标签、但前面有限定的效果（只对部分情况有效）用虚线框
        private static Widget CreateEffectChips(IEnumerable<Influence> influences, int width)
        {
            var all = influences.ToList();
            bool allRestricted = all.Count > 0 && IsRestriction(all[0]);
            var list = new List<(Influence Influence, bool Restricted)>();
            bool restricted = false;
            foreach (Influence influence in all)
            {
                if (IsRestriction(influence))
                {
                    restricted = true;
                }
                else
                {
                    list.Add((influence, restricted && !allRestricted));
                }
            }
            var cell = new HorizontalStackPanel
            {
                Spacing = ChipSpacing
            };
            if (list.Count == 0)
            {
                cell.Widgets.Add(CreateLabel("—", 14, MyraSkin.TextDisabled));
                return cell;
            }
            SpriteFontBase font = TextManager.GetFont(ChipFontSize);
            int used = 0;
            for (int i = 0; i < list.Count; i++)
            {
                var (name, value) = SplitInfluence(list[i].Influence.Description);
                int chipWidth = (int)Math.Ceiling(font.MeasureString(name + (value == null ? "" : " " + value)).X) + 2 * ChipPaddingX;
                // 后面还有效果时要给“+N”留位置；第一个太长也照放，超出的部分被裁掉
                int reserve = i < list.Count - 1 ? ChipSpacing + ChipMoreWidth : 0;
                if (i > 0 && used + ChipSpacing + chipWidth + reserve > width)
                {
                    cell.Widgets.Add(CreateLabel("+" + (list.Count - i), 14, MyraSkin.TextSecondary));
                    break;
                }
                cell.Widgets.Add(CreateChip(name, value, list[i].Restricted));
                used += (i > 0 ? ChipSpacing : 0) + chipWidth;
            }
            return cell;
        }

        private static Widget CreateChip(string name, string value, bool restricted)
        {
            var content = new HorizontalStackPanel
            {
                Spacing = 4,
                VerticalAlignment = VerticalAlignment.Center
            };
            content.Widgets.Add(CreateLabel(name, ChipFontSize, MyraSkin.TextPrimary));
            if (value != null)
            {
                content.Widgets.Add(CreateLabel(value, ChipFontSize, Accent));
            }
            var chip = new Panel
            {
                Height = ChipHeight,
                Padding = new Thickness(ChipPaddingX, 0, ChipPaddingX, 0),
                Background = restricted ? new FrameBrush(ChipColor, LineColor, true) : new SolidBrush(ChipColor)
            };
            chip.Widgets.Add(content);
            return chip;
        }

        #endregion

        #region 详情

        // 详情顶部：小字类别，大字名称
        // 详情顶部：上面一行小字说明，下面大字名称。说明是空的就不占一行，名称往上顶
        private static void AddDetailTop(VerticalStackPanel panel, string caption, string name, Color color, int width)
        {
            if (!string.IsNullOrWhiteSpace(caption))
            {
                panel.Widgets.Add(CreateLabel(caption, 13, MyraSkin.TextSecondary));
            }
            panel.Widgets.Add(CreateWrappedLabel(name, 30, color, width));
        }

        // 小字说明，空的不加，免得留出一行空白
        private static void AddNote(VerticalStackPanel panel, string text, int width)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            panel.Widgets.Add(CreateWrappedLabel(text, 14, MyraSkin.TextSecondary, width));
        }

        // 效果：两级，段标题（要满足的全部限定）→ 效果，名称在左、数值在右，每条下面一条分隔线。
        // 有限定段时所有效果统一缩进一级，段之间多留点空
        private static void AddEffects(VerticalStackPanel panel, IEnumerable<Influence> influences, int width)
        {
            var sections = SplitInfluenceSections(influences);
            if (sections.Count == 0)
            {
                return;
            }
            bool grouped = sections.Any(x => x.Restriction != null);
            int indent = grouped ? EffectIndent : 0;
            panel.Widgets.Add(CreateSpacer(4));
            if (!grouped)
            {
                panel.Widgets.Add(new Panel
                {
                    Height = 1,
                    Background = new SolidBrush(DividerColor)
                });
            }
            for (int i = 0; i < sections.Count; i++)
            {
                if (grouped)
                {
                    panel.Widgets.Add(CreateSpacer(i > 0 ? 12 : 4));
                    panel.Widgets.Add(CreateWrappedLabel(GetSectionTitle(sections[i]), 14, CaptionColor, width));
                }
                foreach (Influence influence in sections[i].Effects)
                {
                    panel.Widgets.Add(Indent(CreateEffectRow(influence, width - indent), indent));
                }
            }
        }

        private static Widget CreateEffectRow(Influence influence, int width)
        {
            var (name, value) = SplitInfluence(influence.Description);
            var row = new Panel
            {
                Width = width,
                Padding = new Thickness(0, EffectRowPadding, 0, EffectRowPadding)
            };
            int valueWidth = 0;
            if (value != null)
            {
                var valueLabel = CreateLabel(value, 16, Accent);
                valueLabel.HorizontalAlignment = HorizontalAlignment.Right;
                valueLabel.VerticalAlignment = VerticalAlignment.Center;
                valueWidth = valueLabel.Measure(new Point(width, int.MaxValue)).X + 10;
                row.Widgets.Add(valueLabel);
            }
            // 拆不出数值时整句里的数值和战法名也上色
            Label nameLabel = CreateWrappedLabel(HighlightEffect(name, MyraSkin.TextPrimary), 16, MyraSkin.TextPrimary, width - valueWidth);
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(nameLabel);

            var holder = new VerticalStackPanel();
            holder.Widgets.Add(row);
            holder.Widgets.Add(new Panel
            {
                Height = 1,
                Background = new SolidBrush(DividerColor)
            });
            return holder;
        }

        #endregion

        #region 共用

        // 一排数值卡片，平分宽度，每张上面小字标题、下面数值
        private static Widget CreateCards(IReadOnlyList<(string Title, string Value, Color Color)> cards, int width)
        {
            int cardWidth = (width - (cards.Count - 1) * KindCardSpacing) / cards.Count;
            var row = new HorizontalStackPanel
            {
                Spacing = KindCardSpacing
            };
            foreach (var (title, value, color) in cards)
            {
                var card = new Panel
                {
                    Width = cardWidth,
                    Height = KindCardHeight,
                    Background = new SolidBrush(ChipColor)
                };
                card.Widgets.Add(CreateCellContent(title, value, 18, color));
                row.Widgets.Add(card);
            }
            return row;
        }

        // 页签里的一页
        private static VerticalStackPanel CreatePage()
        {
            return new VerticalStackPanel
            {
                Spacing = 6
            };
        }

        // 详情里的页签：平分宽度，选中的金字加金线，下面一条分隔线，只显示选中的那页
        private static void AddTabs(VerticalStackPanel panel, IReadOnlyList<(string Title, Widget Page)> pages, int width, int selectedIndex, Action<int> onSelect)
        {
            var bar = new Panel
            {
                Width = width,
                Height = TabHeight
            };
            bar.Widgets.Add(new Panel
            {
                Height = 1,
                VerticalAlignment = VerticalAlignment.Bottom,
                Background = new SolidBrush(DividerColor)
            });
            var row = new HorizontalStackPanel();
            bar.Widgets.Add(row);
            // 各页叠在一起，只显示选中的
            var holder = new Panel();
            int tabWidth = width / pages.Count;
            var labels = new List<Label>();
            var lines = new List<Panel>();
            int current = -1;

            void Select(int index)
            {
                current = index;
                for (int i = 0; i < pages.Count; i++)
                {
                    labels[i].TextColor = i == index ? MyraSkin.TextTitle : MyraSkin.TextSecondary;
                    lines[i].Visible = i == index;
                    pages[i].Page.Visible = i == index;
                }
                onSelect(index);
            }

            for (int i = 0; i < pages.Count; i++)
            {
                var label = CreateLabel(pages[i].Title, 16, MyraSkin.TextSecondary);
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                var line = new Panel
                {
                    Width = tabWidth - 24,
                    Height = 2,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Background = new SolidBrush(Accent)
                };
                var content = new Panel();
                content.Widgets.Add(label);
                content.Widgets.Add(line);
                var button = new Button
                {
                    Content = content,
                    Width = tabWidth,
                    Height = TabHeight,
                    Background = Transparent,
                    FocusedBackground = Transparent,
                    OverBackground = Transparent,
                    PressedBackground = Transparent
                };
                int index = i;
                button.MouseEntered += (sender, args) =>
                {
                    if (current != index)
                    {
                        label.TextColor = MyraSkin.TextPrimary;
                    }
                };
                button.MouseLeft += (sender, args) =>
                {
                    if (current != index)
                    {
                        label.TextColor = MyraSkin.TextSecondary;
                    }
                };
                button.Click += (sender, args) => Select(index);
                labels.Add(label);
                lines.Add(line);
                row.Widgets.Add(button);
                holder.Widgets.Add(pages[i].Page);
            }
            panel.Widgets.Add(bar);
            panel.Widgets.Add(CreateSpacer(4));
            panel.Widgets.Add(holder);
            Select(Math.Clamp(selectedIndex, 0, pages.Count - 1));
        }

        // 单列属性，左边名称、右边数值，每行下面一条分隔线
        private static Widget CreateStatList(IReadOnlyList<(string Name, string Value)> stats, int width)
        {
            var list = new VerticalStackPanel();
            foreach (var stat in stats)
            {
                var row = new Panel
                {
                    Height = GridRowHeight
                };
                row.Widgets.Add(CreateStatCell(stat, width));
                list.Widgets.Add(WithDivider(row));
            }
            return list;
        }

        // 两列属性，每格左边名称、右边数值，每行下面一条分隔线
        private static Widget CreateStatGrid(IReadOnlyList<(string Name, string Value)> stats, int width)
        {
            var grid = new VerticalStackPanel();
            int cellWidth = (width - GridColumnGap) / 2;
            for (int i = 0; i < stats.Count; i += 2)
            {
                var row = new HorizontalStackPanel
                {
                    Spacing = GridColumnGap,
                    Height = GridRowHeight
                };
                row.Widgets.Add(CreateStatCell(stats[i], cellWidth));
                if (i + 1 < stats.Count)
                {
                    row.Widgets.Add(CreateStatCell(stats[i + 1], cellWidth));
                }
                grid.Widgets.Add(WithDivider(row));
            }
            return grid;
        }

        private static Widget CreateStatCell((string Name, string Value) stat, int width)
        {
            var cell = new Panel
            {
                Width = width
            };
            var name = CreateLabel(stat.Name, 14, MyraSkin.TextSecondary);
            name.VerticalAlignment = VerticalAlignment.Center;
            cell.Widgets.Add(name);
            var value = CreateLabel(stat.Value, 15, MyraSkin.TextPrimary);
            value.HorizontalAlignment = HorizontalAlignment.Right;
            value.VerticalAlignment = VerticalAlignment.Center;
            cell.Widgets.Add(value);
            return cell;
        }

        // 一行内容下面加一条分隔线
        private static Widget WithDivider(Widget row)
        {
            var holder = new VerticalStackPanel();
            holder.Widgets.Add(row);
            holder.Widgets.Add(new Panel
            {
                Height = 1,
                Background = new SolidBrush(DividerColor)
            });
            return holder;
        }

        // 浏览时没有具体的人，条件只列出来，不判断满足与否
        private static IEnumerable<KeyValuePair<string, bool?>> Unchecked(IEnumerable<Condition> conditions)
        {
            return conditions.Select(x => new KeyValuePair<string, bool?>(x.Name, null));
        }

        // 能力编号对应的名称，不在范围内时返回null
        private static string GetAbilityName(int ability)
        {
            return ability >= 0 && ability < AbilityNames.Length ? AbilityNames[ability] : null;
        }

        // 一行属性：左边名称，右边数值
        private static void AddStat(VerticalStackPanel panel, string name, string value, int width)
        {
            var row = new HorizontalStackPanel
            {
                Height = StatRowHeight
            };
            row.Widgets.Add(CreateFixedLabel(name, MyraSkin.TextSecondary, StatLabelWidth, TextHorizontalAlignment.Left));
            row.Widgets.Add(CreateFixedLabel(value, MyraSkin.TextPrimary, width - StatLabelWidth, TextHorizontalAlignment.Right));
            panel.Widgets.Add(row);
        }

        private static Label CreateFixedLabel(string text, Color color, int width, TextHorizontalAlignment align)
        {
            return new Label
            {
                Text = text,
                Font = TextManager.GetFont(15),
                TextColor = color,
                Width = width,
                TextAlign = align,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        // 持有者只占一行：人数和“查看”，点了再弹出持有者列表
        private static void AddHolders(VerticalStackPanel panel, string name, IEnumerable<Person> persons, int width)
        {
            var list = persons.OrderBy(x => x.ID).ToList();
            panel.Widgets.Add(CreateDetailHeader("持有者", width));
            var row = new HorizontalStackPanel
            {
                Spacing = 12,
                Height = LinkHeight
            };
            var count = CreateLabel(list.Count + " 人", 15, list.Count == 0 ? MyraSkin.TextSecondary : MyraSkin.TextPrimary);
            count.VerticalAlignment = VerticalAlignment.Center;
            row.Widgets.Add(count);
            if (list.Count > 0)
            {
                row.Widgets.Add(CreateLink("查看 ›", () => ShowHolders("「" + name + "」持有者", list)));
            }
            panel.Widgets.Add(row);
        }

        // 持有者列表：姓名、势力和五维，右边是人物简要信息，可以再打开人物详情
        private static void ShowHolders(string title, List<Person> persons)
        {
            var columns = new List<InfoColumn<Person>>
            {
                new InfoColumn<Person>("姓名", PersonNameWidth, x => x.Name, x => x.Name),
                new InfoColumn<Person>("势力", 90, GetFactionName, GetFactionName)
            };
            columns.AddRange(CreateAbilityColumns());
            InfoListDialog<Person>.Show(title, persons, columns, null, ShowPersonDetail);
        }

        private static string GetFactionName(Person person)
        {
            return person.BelongedFaction?.Name ?? "在野";
        }

        // 可点的文字：金字细边框，鼠标移上去有底色
        private static Button CreateLink(string text, Action onClick)
        {
            var label = new Label
            {
                Text = text,
                Font = TextManager.GetFont(15),
                TextColor = Accent,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            int width = label.Measure(new Point(int.MaxValue, LinkHeight)).X + 16;
            Button button = CreateFlatButton(label, width, LinkHeight, new FrameBrush(ChipColor, GoldDim, false));
            button.Click += (sender, args) => onClick();
            return button;
        }

        // Myra没有自动换行的容器，按控件量出的宽度手动排成几行
        private static void AddFlow(VerticalStackPanel panel, IEnumerable<Widget> widgets, int width)
        {
            HorizontalStackPanel line = null;
            int lineWidth = 0;
            foreach (Widget widget in widgets)
            {
                int w = widget.Width ?? widget.Measure(new Point(width, int.MaxValue)).X;
                if (line == null || lineWidth + FlowSpacing + w > width)
                {
                    line = new HorizontalStackPanel
                    {
                        Spacing = FlowSpacing
                    };
                    panel.Widgets.Add(line);
                    lineWidth = w;
                }
                else
                {
                    lineWidth += FlowSpacing + w;
                }
                line.Widgets.Add(widget);
            }
        }

        #endregion
    }
}
