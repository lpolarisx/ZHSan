using Extensions;
using GameEnums;
using GameManager;
using GameObjects;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WorldOfTheThreeKingdoms.GameScreens
{
    /// <summary>
    /// 人物、城池、部队、编队、兵种、军区、宝物列表在“概览”后面的视图页签。
    /// 内容对应旧版列表的各个页签，每列只在设置的列表详细程度（TabListDetailLevel）够高时显示，和旧版一样
    /// </summary>
    internal static partial class InformationLists
    {
        // 各列表名称列的宽度，按名称一般有几个字定，再长的截短。概览和各视图共用
        private const int PersonNameWidth = 84;
        private const int ArchitectureNameWidth = 84;
        private const int TroopNameWidth = 120;
        private const int MilitaryNameWidth = 120;
        private const int MilitaryKindNameWidth = 100;
        private const int SectionNameWidth = 110;
        private const int TreasureNameWidth = 120;

        private static readonly Regex LeadingNumber = new Regex(@"^-?\d+(\.\d+)?", RegexOptions.Compiled);

        /// <summary>
        /// 视图里的一列和它要求的详细程度
        /// </summary>
        private sealed class ViewColumn<T>
        {
            public InfoColumn<T> Column;
            public int Level;
        }

        // 一列：文字按值的类型显示（是否显示○×，枚举显示中文名），按值本身排序
        private static ViewColumn<T> Col<T, V>(string header, int width, Func<T, V> value, int level = 1)
        {
            Func<T, IComparable> sortKey = x => value(x) is IComparable comparable ? comparable : FormatValue(value(x));
            return new ViewColumn<T> { Column = new InfoColumn<T>(header, width, x => FormatValue(value(x)), sortKey), Level = level };
        }

        // 文字里带数字的列（例如“12/30”“35岁”）按开头的数字排序，没有数字的排在最前
        private static ViewColumn<T> NumCol<T>(string header, int width, Func<T, string> value, int level = 1)
        {
            return new ViewColumn<T>
            {
                Column = new InfoColumn<T>(header, width, x => value(x) ?? "----", x => ParseLeadingNumber(value(x))),
                Level = level
            };
        }

        private static double ParseLeadingNumber(string text)
        {
            Match match = LeadingNumber.Match(text ?? "");
            return match.Success ? double.Parse(match.Value) : double.MinValue;
        }

        private static string FormatValue(object value)
        {
            switch (value)
            {
                case null:
                    return "----";
                case bool flag:
                    return flag ? "○" : "×";
                case Enum enumValue:
                    return enumValue.GetDescription();
                case float number:
                    return number.ToString("0.##");
                case double number:
                    return number.ToString("0.##");
                default:
                    return value.ToString();
            }
        }

        /// <summary>
        /// 一个视图：第一列是名称，后面是给出的列（null的跳过）。详细程度不够的列不显示，一列都不剩时整个视图不要
        /// </summary>
        private static InfoView<T> View<T>(string name, Func<T, string> nameText, int nameWidth, params ViewColumn<T>[] columns)
        {
            int level = Session.GlobalVariables.TabListDetailLevel;
            List<InfoColumn<T>> shown = columns.Where(x => x != null && x.Level <= level).Select(x => x.Column).ToList();
            if (shown.Count == 0)
            {
                return null;
            }
            shown.Insert(0, new InfoColumn<T>("名称", nameWidth, nameText, x => nameText(x)));
            return new InfoView<T>(name, shown);
        }

        private static List<InfoView<T>> Views<T>(params InfoView<T>[] views)
        {
            return views.Where(x => x != null).ToList();
        }

        #region 人物

        private static bool IsServing(Person person)
        {
            return person.Status == PersonStatus.Normal || person.Status == PersonStatus.Moving;
        }

        // 哪一年仕官。剧本里开局就在势力里的人物没有记录（YearJoin为0），写“开局前”；不在势力里的写“----”
        private static string GetJoinYearText(Person person)
        {
            if (!IsServing(person))
            {
                return "----";
            }
            return person.YearJoin > 0 ? person.YearJoin + "年" : "开局前";
        }

        // 技能有没有：表头用技能名，剧本里没有这个技能就不显示这一列
        private static ViewColumn<Person>[] SkillCols(params int[] ids)
        {
            var skills = Session.Current.Scenario.GameCommonData.AllSkills;
            return ids.Where(skills.ContainsKey)
                .Select(id => Col<Person, bool>(skills[id].Name, 44, x => x.HasSkill(id), 5))
                .ToArray();
        }

        private static ViewColumn<Person> TitleCol(string header, int kind)
        {
            return Col<Person, string>(header, 110, x => x.TitleName(kind), 3);
        }

        private static ViewColumn<Person> TreasureGroupCol(string header, int group)
        {
            return Col<Person, bool>(header, 44, x => x.HasTreasureforGroup(group), 6);
        }

        private static ViewColumn<Person> TreasureValueCol(string header, int kind)
        {
            return Col<Person, float>(header, 44, x => x.InfluenceKindValueByTreasure(kind), 6);
        }

        /// <summary>
        /// 旧版“基本”页签的列，接在人物概览的姓名、势力、所在、五维后面。势力、所在概览里已有，这里不再重复
        /// </summary>
        private static IEnumerable<InfoColumn<Person>> CreatePersonBasicColumns()
        {
            int level = Session.GlobalVariables.TabListDetailLevel;
            var columns = new[]
            {
                Col<Person, string>("所属军区", 90, x => x.SectionString),
                Col<Person, string>("负伤", 56, x => x.InjuryString, 2),
                Col<Person, string>("途中", 56, x => x.Travel, 4),
                Col<Person, string>("工作", 60, x => x.WorkKindString, 2),
                Col<Person, string>("任务", 60, x => x.OutsideTaskString),
                NumCol<Person>("期限", 56, x => x.OutsideTaskDaysString, 2),
                Col<Person, int>("疲累度", 56, x => x.Tiredness, 4),
                Col<Person, string>("野武将", 56, x => x.OfficerString, 2),
                Col<Person, bool>("女官", 44, x => x.NvGuan)
            };
            return columns.Where(x => x.Level <= level).Select(x => x.Column);
        }

        private static List<InfoView<Person>> CreatePersonViews()
        {
            Func<Person, string> name = x => x.Name;
            int nameWidth = PersonNameWidth;
            return Views(                View("能力", name, nameWidth,
                    Col<Person, int>("综合实力", 72, x => x.Merit, 4),
                    Col<Person, int>("主将战斗", 72, x => x.FightingForce, 4),
                    Col<Person, int>("副将战斗", 72, x => x.SubFightingForce, 4),
                    Col<Person, int>("武学", 44, x => x.NormalStrength, 2),
                    Col<Person, int>("将略", 44, x => x.NormalCommand, 2),
                    Col<Person, int>("谋略", 44, x => x.NormalIntelligence, 2),
                    Col<Person, int>("政理", 44, x => x.NormalPolitics, 2),
                    Col<Person, int>("风度", 44, x => x.NormalGlamour, 2),
                    Col<Person, int>("勇猛", 44, x => x.Braveness, 2),
                    Col<Person, int>("冷静", 44, x => x.Calmness, 2),
                    Col<Person, int>("功绩", 60, x => x.OfficerMerit, 3),
                    Col<Person, string>("培育方针", 72, x => x.TrainPolicyString, 2)),
                View("个人", name, nameWidth,
                    Col<Person, string>("字", 50, x => x.CalledName, 2),
                    Col<Person, string>("性别", 44, x => x.SexString),
                    Col<Person, string>("性格", 50, x => x.CharacterString, 5),
                    NumCol<Person>("年龄", 44, x => x.DisplayedAge, 2),
                    Col<Person, int>("志向", 44, x => x.Ideal, 9),
                    Col<Person, string>("出仕志向考虑", 110, x => x.IdealTendencyString, 9),
                    Col<Person, int>("忠诚度", 56, x => x.Loyalty, 8),
                    Col<Person, int>("宝物", 44, x => x.TreasureCount, 3),
                    new ViewColumn<Person>
                    {
                        Column = new InfoColumn<Person>("仕官年", 64, GetJoinYearText, x => IsServing(x) ? x.YearJoin : int.MinValue),
                        Level = 3
                    },
                    Col<Person, int>("身价", 64, x => x.UntiredMerit, 4),
                    Col<Person, int>("银两", 56, x => x.Fund, 9),
                    Col<Person, int>("名声", 64, x => x.Reputation, 2),
                    Col<Person, int>("善名", 56, x => x.Karma, 7)),
                View("内政", name, nameWidth,
                    Col<Person, string>("工作", 60, x => x.WorkKindString, 2),
                    Col<Person, int>("农业能力", 72, x => x.NormalAgricultureAbility, 3),
                    Col<Person, int>("商业能力", 72, x => x.NormalCommerceAbility, 3),
                    Col<Person, int>("技术能力", 72, x => x.NormalTechnologyAbility, 3),
                    Col<Person, int>("统治能力", 72, x => x.NormalDominationAbility, 3),
                    Col<Person, int>("民心能力", 72, x => x.NormalMoraleAbility, 3),
                    Col<Person, int>("耐久能力", 72, x => x.NormalEnduranceAbility, 3),
                    Col<Person, bool>("内政不需资金", 100, x => x.InternalNoFundNeeded, 6)),
                View("军事", name, nameWidth,
                    Col<Person, int>("主将战斗", 72, x => x.FightingForce, 4),
                    Col<Person, int>("副将战斗", 72, x => x.SubFightingForce, 4),
                    Col<Person, string>("工作", 60, x => x.WorkKindString, 2),
                    Col<Person, int>("训练能力", 72, x => x.NormalTrainingAbility, 3),
                    Col<Person, int>("补充能力", 72, x => x.NormalRecruitmentAbility, 3),
                    Col<Person, bool>("有军队追随", 86, x => x.HasFollowingArmy, 3),
                    Col<Person, bool>("有军队带领", 86, x => x.HasLeadingArmy, 2),
                    Col<Person, bool>("副将", 44, x => x.IsBeingTroopPerson, 4)),
                View("策略", name, nameWidth,
                    Col<Person, string>("任务", 60, x => x.OutsideTaskString),
                    NumCol<Person>("期限", 56, x => x.OutsideTaskDaysString, 2),
                    Col<Person, int>("情报能力", 72, x => x.InformationAbility, 3),
                    Col<Person, int>("间谍能力", 72, x => x.SpyAbility, 3),
                    Col<Person, int>("破坏能力", 72, x => x.DestroyAbility, 3),
                    Col<Person, int>("煽动能力", 72, x => x.InstigateAbility, 3),
                    Col<Person, int>("流言能力", 72, x => x.GossipAbility, 3),
                    Col<Person, int>("搜索能力", 72, x => x.SearchAbility, 3),
                    Col<Person, int>("说服能力", 72, x => x.ConvinceAbility, 3),
                    Col<Person, int>("劫狱能力", 72, x => x.JailBreakAbility, 3),
                    Col<Person, int>("暗杀能力", 72, x => x.AssassinateAbility, 3)),
                View("称号", name, nameWidth,
                    TitleCol("称号", 1),
                    TitleCol("氏族", 55),
                    TitleCol("君主", 3),
                    TitleCol("武将", 20),
                    TitleCol("文官", 21),
                    TitleCol("爵位", 10),
                    TitleCol("兵科", 6),
                    TitleCol("军师", 7),
                    TitleCol("统领", 4),
                    TitleCol("合称", 2),
                    TitleCol("女官", 5)),
                View("技能特技", name, nameWidth,
                    Col<Person, int>("技能数", 56, x => x.SkillCount, 7),
                    Col<Person, string>("特技", 0, x => string.Join("、", x.Stunts.Values.Select(s => s.Name)), 3)),
                View("能力经验", name, nameWidth,
                    Col<Person, int>("内政经验", 72, x => x.InternalExperience, 4),
                    Col<Person, int>("武学经验", 72, x => x.StrengthExperience, 4),
                    Col<Person, int>("将略经验", 72, x => x.CommandExperience, 4),
                    Col<Person, int>("谋略经验", 72, x => x.IntelligenceExperience, 4),
                    Col<Person, int>("政理经验", 72, x => x.PoliticsExperience, 4),
                    Col<Person, int>("风度经验", 72, x => x.GlamourExperience, 4)),
                View("战斗经验", name, nameWidth,
                    Col<Person, int>("策略经验", 72, x => x.TacticsExperience, 4),
                    Col<Person, int>("计略经验", 72, x => x.StratagemExperience, 4),
                    Col<Person, int>("步兵经验", 72, x => x.BubingExperience, 4),
                    Col<Person, int>("弩兵经验", 72, x => x.NubingExperience, 4),
                    Col<Person, int>("骑兵经验", 72, x => x.QibingExperience, 4),
                    Col<Person, int>("水军经验", 72, x => x.ShuijunExperience, 4),
                    Col<Person, int>("器械经验", 72, x => x.QixieExperience, 4),
                    Col<Person, int>("总士兵伤害", 86, x => x.TroopDamageDealt, 7),
                    Col<Person, int>("总受士兵伤害", 100, x => x.TroopBeDamageDealt, 7),
                    Col<Person, int>("总建筑伤害", 86, x => x.ArchitectureDamageDealt, 7)),
                View("战斗次数", name, nameWidth,
                    Col<Person, int>("击破", 44, x => x.RoutCount, 4),
                    Col<Person, int>("被击破", 56, x => x.RoutedCount, 4),
                    Col<Person, int>("反叛数", 56, x => x.RebelCount, 7),
                    Col<Person, int>("处斩数", 56, x => x.ExecuteCount, 7),
                    Col<Person, int>("逃跑数", 56, x => x.FleeCount, 7),
                    Col<Person, int>("被俘次数", 72, x => x.HeldCaptiveCount, 7),
                    Col<Person, int>("俘获将领次数", 100, x => x.CaptiveCount, 7),
                    Col<Person, int>("致战死数", 72, x => x.OfficerKillCount, 7),
                    Col<Person, int>("计略成功次数", 100, x => x.StratagemSuccessCount, 7),
                    Col<Person, int>("计略失败次数", 100, x => x.StratagemFailCount, 7),
                    Col<Person, int>("计略中计次数", 100, x => x.StratagemBeSuccessCount, 7),
                    Col<Person, int>("计略阻挡次数", 100, x => x.StratagemBeFailCount, 7)),
                View("人物关系", name, nameWidth,
                    Col<Person, string>("父亲", 80, x => x.FatherName, 4),
                    Col<Person, string>("母亲", 80, x => x.MotherName, 4),
                    Col<Person, string>("配偶", 80, x => x.SpouseName, 4),
                    Col<Person, string>("义兄弟", 160, x => x.BrotherName, 4),
                    Col<Person, int>("子女数量", 72, x => x.NumberOfChildren, 4),
                    Col<Person, string>("已怀孕", 56, x => x.FoundPregnantString, 2),
                    Col<Person, int>("怀孕月数", 72, x => x.PregnantDayForShowing, 3)),
                View("关系武将", name, nameWidth,
                    Col<Person, int>("对君主", 56, x => x.RelationToLeader, 8),
                    Col<Person, string>("关系武将", 0, x => x.PersonSpecialRelationString, 8)),
                View("行军步兵", name, nameWidth, SkillCols(20, 21, 22, 23, 24, 25, 26, 30, 31, 32, 36, 33, 34, 35)),
                View("弩兵骑兵", name, nameWidth, SkillCols(40, 41, 42, 46, 43, 44, 45, 50, 51, 52, 56, 53, 54, 55)),
                View("水战兵器", name, nameWidth, SkillCols(60, 61, 62, 63, 64, 65, 66, 70, 71, 72, 73, 74, 75, 76)),
                View("练兵计略", name, nameWidth, SkillCols(80, 81, 82, 83, 84, 85, 86, 90, 91, 92, 93, 94, 95, 96, 110, 111)),
                View("宝物", name, nameWidth,
                    TreasureGroupCol("武器", 1100),
                    TreasureGroupCol("马", 300),
                    TreasureGroupCol("政书", 1300),
                    TreasureGroupCol("乐器", 2900),
                    TreasureGroupCol("药", 2001),
                    TreasureGroupCol("器具", 3200),
                    TreasureGroupCol("头盔", 4100),
                    TreasureGroupCol("铠甲", 4000),
                    TreasureGroupCol("兵书", 1200),
                    TreasureGroupCol("印玺", 3214),
                    TreasureValueCol("将略", 6000),
                    TreasureValueCol("武力", 6010),
                    TreasureValueCol("智力", 6020),
                    TreasureValueCol("政理", 6030),
                    TreasureValueCol("风度", 6040),
                    Col<Person, bool>("马", 44, x => x.HasInfluenceKind(5110), 6),
                    TreasureValueCol("抗暴", 2330)));
        }

        #endregion

        #region 城池

        private static List<InfoView<Architecture>> CreateArchitectureViews()
        {
            Func<Architecture, string> name = x => x.Name;
            int nameWidth = ArchitectureNameWidth;
            return Views(
                View("基本", name, nameWidth,
                    Col<Architecture, string>("势力", 80, x => x.FactionString),
                    Col<Architecture, string>("所属军区", 90, x => x.SectionString),
                    Col<Architecture, int>("人口", 80, x => x.Population, 2),
                    Col<Architecture, int>("资金", 72, x => x.Fund, 2),
                    Col<Architecture, int>("军粮", 80, x => x.Food, 2),
                    Col<Architecture, int>("待命中", 56, x => x.IdlingPersonCount, 3),
                    Col<Architecture, int>("武将", 44, x => x.PersonCount, 3),
                    Col<Architecture, int>("移动中", 56, x => x.MovingPersonCount, 3),
                    Col<Architecture, int>("在野", 44, x => x.NoFactionPersonCount, 4),
                    Col<Architecture, string>("县令", 80, x => x.MayorName, 5)),
                View("资源", name, nameWidth,
                    Col<Architecture, string>("势力", 80, x => x.FactionString),
                    Col<Architecture, int>("人口", 80, x => x.Population, 2),
                    Col<Architecture, int>("资金", 72, x => x.Fund, 2),
                    Col<Architecture, int>("军粮", 80, x => x.Food, 2),
                    Col<Architecture, int>("正送到资金", 86, x => x.FundInPack, 2),
                    Col<Architecture, int>("正送到军粮", 86, x => x.FoodInPack, 2),
                    NumCol<Architecture>("收入缩减率", 86, x => x.FactionInternalSurplusRatePercentString, 6)),
                View("内政", name, nameWidth,
                    Col<Architecture, float>("内政效率", 72, x => x.CurrentRateOfInternal, 3),
                    Col<Architecture, int>("农业", 56, x => x.Agriculture, 2),
                    Col<Architecture, int>("商业", 56, x => x.Commerce, 2),
                    Col<Architecture, int>("技术", 56, x => x.Technology, 2),
                    Col<Architecture, int>("统治", 56, x => x.Domination, 2),
                    Col<Architecture, int>("民心", 56, x => x.Morale, 2),
                    Col<Architecture, int>("耐久", 56, x => x.Endurance, 2),
                    Col<Architecture, double>("人口增长率(%)", 110, x => x.PopulationDevelopingRateString, 4),
                    NumCol<Architecture>("资金收入", 80, x => x.ExpectedFundString, 3),
                    NumCol<Architecture>("军粮收入", 80, x => x.ExpectedFoodString, 3)),
                View("军备", name, nameWidth,
                    Col<Architecture, int>("兵役人口", 80, x => x.MilitaryPopulation, 2),
                    Col<Architecture, int>("军队规模", 72, x => x.ArmyScale, 2),
                    Col<Architecture, int>("军队数量", 80, x => x.ArmyQuantity, 3),
                    Col<Architecture, int>("编队数量", 72, x => x.MilitaryCount, 3),
                    Col<Architecture, int>("粮道维持费", 86, x => x.RoutewayActiveCost, 3),
                    NumCol<Architecture>("粮草衰减率", 86, x => x.FoodReduceDayRateString, 4),
                    Col<Architecture, int>("情报维持费", 86, x => x.InformationDayCost, 5)),
                View("一般", name, nameWidth,
                    Col<Architecture, string>("州域", 64, x => x.StateString, 3),
                    Col<Architecture, string>("所属地区", 72, x => x.RegionString),
                    Col<Architecture, string>("种类", 50, x => x.KindString),
                    Col<Architecture, int>("规模", 44, x => x.JianzhuGuimo),
                    Col<Architecture, string>("州效果", 80, x => x.StateEffectString, 4),
                    Col<Architecture, string>("地区效果", 80, x => x.RegionEffectString, 4),
                    Col<Architecture, string>("是否战略要冲", 100, x => x.IsStrategicCenterString, 4),
                    Col<Architecture, string>("前线", 44, x => x.FrontLineString, 5),
                    Col<Architecture, int>("视野范围", 72, x => x.ViewDistance, 4)),
                View("上限", name, nameWidth,
                    Col<Architecture, int>("人口上限", 80, x => x.PopulationCeiling, 2),
                    Col<Architecture, int>("农业上限", 72, x => x.AgricultureCeiling, 2),
                    Col<Architecture, int>("商业上限", 72, x => x.CommerceCeiling, 2),
                    Col<Architecture, int>("技术上限", 72, x => x.TechnologyCeiling, 2),
                    Col<Architecture, int>("统治上限", 72, x => x.DominationCeiling, 2),
                    Col<Architecture, int>("民心上限", 72, x => x.MoraleCeiling, 2),
                    Col<Architecture, int>("耐久上限", 72, x => x.EnduranceCeiling, 2),
                    Col<Architecture, int>("资金上限", 80, x => x.FundCeiling, 2),
                    Col<Architecture, int>("粮草上限", 80, x => x.FoodCeiling, 2)),
                View("设施", name, nameWidth,
                    Col<Architecture, int>("规模", 44, x => x.JianzhuGuimo),
                    NumCol<Architecture>("设施空间", 72, x => x.FacilityPositionString, 2),
                    Col<Architecture, string>("在建设施", 90, x => x.BuildingFacilityName, 2),
                    Col<Architecture, int>("剩余天数", 72, x => x.BuildingDaysLeft, 3),
                    NumCol<Architecture>("妃子空间", 72, x => x.meinvkongjianzifu, 2)),
                View("灾难", name, nameWidth,
                    Col<Architecture, string>("灾难种类", 80, x => x.zainanming, 2),
                    NumCol<Architecture>("剩余天数", 72, x => x.zainanshengyutianshu, 9)),
                View("运输", name, nameWidth,
                    Col<Architecture, string>("正送到资金", 0, x => x.FundPackString, 5),
                    Col<Architecture, string>("正送到军粮", 360, x => x.FoodPackString, 5)));
        }

        #endregion

        #region 部队

        private static List<InfoView<Troop>> CreateTroopViews()
        {
            Func<Troop, string> name = x => x.DisplayName;
            int nameWidth = TroopNameWidth;
            return Views(
                View("基本", name, nameWidth,
                    Col<Troop, string>("兵种", 80, x => x.KindString),
                    Col<Troop, string>("势力", 80, x => x.FactionString),
                    Col<Troop, string>("所属军区", 90, x => x.SectionString),
                    Col<Troop, int>("人物数量", 72, x => x.PersonCount, 3),
                    Col<Troop, int>("士兵数量", 72, x => x.Quantity, 2),
                    Col<Troop, int>("军队士气", 72, x => x.Morale, 2),
                    Col<Troop, int>("军队战意", 72, x => x.Combativity, 2),
                    Col<Troop, int>("伤兵数量", 72, x => x.InjuryQuantity, 2),
                    Col<Troop, int>("疲累度", 56, x => x.Tiredness, 4)),
                View("能力", name, nameWidth,
                    Col<Troop, int>("部队武学", 72, x => x.TroopStrength, 3),
                    Col<Troop, int>("部队将略", 72, x => x.TroopCommand, 3),
                    Col<Troop, int>("部队谋略", 72, x => x.TroopIntelligence, 3),
                    Col<Troop, string>("战斗称号", 110, x => x.CombatTitleString, 3),
                    Col<Troop, int>("战斗力", 64, x => x.FightingForce, 4),
                    NumCol<Troop>("口粮天数", 72, x => x.RationDaysString, 3),
                    Col<Troop, int>("粮草总量", 80, x => x.Food, 3),
                    Col<Troop, string>("当前行动", 80, x => x.CurrentCombatAction, 6)),
                View("战斗", name, nameWidth,
                    Col<Troop, int>("行动速率", 72, x => x.Speed, 4),
                    Col<Troop, int>("攻击力", 64, x => x.Offence, 3),
                    Col<Troop, int>("防御力", 64, x => x.Defence, 3),
                    Col<Troop, int>("攻击范围", 72, x => x.OffenceRadius, 4),
                    Col<Troop, string>("斜向攻击", 72, x => x.ObliqueOffenceString, 4),
                    Col<Troop, int>("暴击几率", 72, x => x.CriticalStrikeChance, 3),
                    Col<Troop, int>("抗暴几率", 72, x => x.AntiCriticalStrikeChance, 3),
                    Col<Troop, int>("暴击致乱", 72, x => x.ChaosAfterCriticalStrikeChance, 3),
                    Col<Troop, int>("抗围几率", 72, x => x.AvoidSurroundedChance, 3),
                    Col<Troop, int>("围攻致乱", 72, x => x.ChaosAfterSurroundAttackChance, 3)),
                View("计略", name, nameWidth,
                    Col<Troop, int>("部队谋略", 72, x => x.TroopIntelligence, 3),
                    Col<Troop, int>("计略范围", 72, x => x.StratagemRadius, 4),
                    Col<Troop, string>("斜向计略", 72, x => x.ObliqueStratagemString, 4),
                    Col<Troop, int>("计略成功率增加", 110, x => x.StratagemChanceIncrement, 3),
                    Col<Troop, int>("计略抵抗率增加", 110, x => x.AntiStratagemChanceIncrement, 3),
                    Col<Troop, int>("计略致乱几率", 100, x => x.ChaosAfterStratagemSuccessChance, 3)),
                View("行军", name, nameWidth,
                    Col<Troop, int>("行动力", 56, x => x.RealMovability, 4),
                    Col<Troop, int>("平原", 44, x => x.PlainAdaptability, 4),
                    Col<Troop, int>("草原", 44, x => x.GrasslandAdaptability, 4),
                    Col<Troop, int>("森林", 44, x => x.ForrestAdaptability, 4),
                    Col<Troop, int>("湿地", 44, x => x.MarshAdaptability, 4),
                    Col<Troop, int>("山地", 44, x => x.MountainAdaptability, 4),
                    Col<Troop, int>("水域", 44, x => x.WaterAdaptability, 4),
                    Col<Troop, int>("峻岭", 44, x => x.RidgeAdaptability, 4),
                    Col<Troop, int>("荒地", 44, x => x.WastelandAdaptability, 4),
                    Col<Troop, int>("栈道", 44, x => x.DesertAdaptability, 4),
                    Col<Troop, int>("瘴气林", 56, x => x.CliffAdaptability, 4)),
                View("乘数", name, nameWidth,
                    Col<Troop, float>("平原", 44, x => x.PlainRate, 4),
                    Col<Troop, float>("草原", 44, x => x.GrasslandRate, 4),
                    Col<Troop, float>("森林", 44, x => x.ForrestRate, 4),
                    Col<Troop, float>("湿地", 44, x => x.MarshRate, 4),
                    Col<Troop, float>("山地", 44, x => x.MountainRate, 4),
                    Col<Troop, float>("水域", 44, x => x.WaterRate, 4),
                    Col<Troop, float>("峻岭", 44, x => x.RidgeRate, 4),
                    Col<Troop, float>("荒地", 44, x => x.WastelandRate, 4),
                    Col<Troop, float>("栈道", 44, x => x.DesertRate, 4),
                    Col<Troop, float>("瘴气林", 56, x => x.CliffRate, 4)),
                View("特色", name, nameWidth,
                    Col<Troop, int>("军队经验", 72, x => x.Experience, 4),
                    Col<Troop, int>("伤兵率", 56, x => x.InjuryChance, 4),
                    Col<Troop, float>("建筑伤害率", 86, x => x.ArchitectureDamageRate, 4),
                    Col<Troop, float>("建筑反击率", 86, x => x.ArchitectureCounterDamageRate, 4),
                    Col<Troop, int>("视野范围", 72, x => x.ViewRadius, 4),
                    Col<Troop, string>("斜向视野", 72, x => x.ObliqueViewString, 4),
                    Col<Troop, float>("受火伤率", 72, x => x.FireDamageRate, 4),
                    Col<Troop, int>("编队上限", 72, x => x.RecruitLimit, 4),
                    Col<Troop, string>("误伤", 44, x => x.AccidentalInjuryString, 4)),
                View("目标", name, nameWidth,
                    Col<Troop, string>("目标部队", 80, x => x.TargetTroopString, 7),
                    Col<Troop, string>("目标建筑", 80, x => x.TargetArchitectureString, 7),
                    Col<Troop, string>("意愿部队", 80, x => x.WillTroopString, 7),
                    Col<Troop, string>("意愿建筑", 80, x => x.WillArchitectureString, 7),
                    Col<Troop, string>("出发建筑", 80, x => x.StartArchitectureString, 6),
                    Col<Troop, string>("命令", 56, x => x.mingling, 6),
                    Col<Troop, string>("意愿", 72, x => x.Will.ToString(), 6),
                    Col<Troop, string>("目标位置", 100, x => x.RealDestination.ToString(), 6)));
        }

        #endregion

        #region 编队

        /// <param name="showLocation">城池里的编队都在这座城，不显示“所在”</param>
        private static List<InfoView<Military>> CreateMilitaryViews(bool showLocation)
        {
            Func<Military, string> name = x => x.Name;
            int nameWidth = MilitaryNameWidth;
            ViewColumn<Military> Location() => showLocation ? Col<Military, string>("所在", 80, x => x.LocationString) : null;
            return Views(
                View("基本", name, nameWidth,
                    NumCol<Military>("经验", 80, x => x.ExperienceWithLimit, 4),
                    Col<Military, string>("当前将领", 72, x => x.LeaderName, 2),
                    Location(),
                    Col<Military, string>("训练中", 56, x => x.TrainingString, 2),
                    Col<Military, string>("补充中", 56, x => x.RecruitmentString, 2),
                    Col<Military, int>("兵数", 64, x => x.Quantity, 2),
                    Col<Military, int>("士气", 44, x => x.Morale, 2),
                    Col<Military, int>("战意", 44, x => x.Combativity, 2),
                    Col<Military, int>("伤兵数量", 72, x => x.InjuryQuantity, 2)),
                View("将领", name, nameWidth,
                    Col<Military, string>("兵种", 72, x => x.KindString),
                    Location(),
                    Col<Military, string>("追随将领", 72, x => x.FollowedLeaderName, 3),
                    Col<Military, bool>("追随将领在城", 100, x => x.FollowedLeaderAvailable, 6),
                    Col<Military, string>("当前将领", 72, x => x.LeaderName, 2),
                    Col<Military, bool>("将领在城", 72, x => x.LeaderAvailable, 6),
                    Col<Military, bool>("副将在城", 72, x => x.AllPersonsAvailable, 6),
                    Col<Military, int>("将领信任度", 86, x => x.LeaderExperience, 4)),
                View("战斗", name, nameWidth,
                    Col<Military, string>("兵种", 72, x => x.KindString),
                    Col<Military, string>("实际兵种", 72, x => x.RealKind, 2),
                    Col<Military, int>("攻击力", 64, x => x.Offence, 3),
                    Col<Military, int>("防御力", 64, x => x.Defence, 3),
                    Col<Military, int>("单位数", 56, x => x.Scales, 2),
                    Col<Military, int>("疲累度", 56, x => x.Tiredness, 4)),
                View("特色", name, nameWidth,
                    Col<Military, float>("建筑伤害率", 86, x => x.ArchitectureDamageRate, 4),
                    Col<Military, float>("建筑反击率", 86, x => x.ArchitectureCounterDamageRate, 4),
                    Col<Military, int>("视野范围", 72, x => x.ViewRadius, 4),
                    Col<Military, string>("斜向视野", 72, x => x.ObliqueViewString, 4),
                    Col<Military, float>("受火伤率", 72, x => x.FireDamageRate, 4),
                    Col<Military, int>("编队上限", 72, x => x.RecruitLimit, 4)),
                View("适性", name, nameWidth,
                    Col<Military, int>("行动力", 56, x => x.Movability, 4),
                    Col<Military, int>("平原", 44, x => x.PlainAdaptability, 4),
                    Col<Military, int>("草原", 44, x => x.GrasslandAdaptability, 4),
                    Col<Military, int>("森林", 44, x => x.ForrestAdaptability, 4),
                    Col<Military, int>("湿地", 44, x => x.MarshAdaptability, 4),
                    Col<Military, int>("山地", 44, x => x.MountainAdaptability, 4),
                    Col<Military, int>("水域", 44, x => x.WaterAdaptability, 4),
                    Col<Military, int>("峻岭", 44, x => x.RidgeAdaptability, 4),
                    Col<Military, int>("荒地", 44, x => x.WastelandAdaptability, 4),
                    Col<Military, int>("栈道", 44, x => x.DesertAdaptability, 4),
                    Col<Military, int>("瘴气林", 56, x => x.CliffAdaptability, 4)),
                View("乘数", name, nameWidth,
                    Col<Military, float>("平原", 44, x => x.PlainRate, 4),
                    Col<Military, float>("草原", 44, x => x.GrasslandRate, 4),
                    Col<Military, float>("森林", 44, x => x.ForrestRate, 4),
                    Col<Military, float>("湿地", 44, x => x.MarshRate, 4),
                    Col<Military, float>("山地", 44, x => x.MountainRate, 4),
                    Col<Military, float>("水域", 44, x => x.WaterRate, 4),
                    Col<Military, float>("峻岭", 44, x => x.RidgeRate, 4),
                    Col<Military, float>("荒地", 44, x => x.WastelandRate, 4),
                    Col<Military, float>("栈道", 44, x => x.DesertRate, 4),
                    Col<Military, float>("瘴气林", 56, x => x.CliffRate, 4)),
                View("介绍", name, nameWidth,
                    Col<Military, string>("介绍", 0, x => x.Description, 3)),
                View("运输", name, nameWidth,
                    Col<Military, string>("兵种", 72, x => x.KindString),
                    Location(),
                    Col<Military, int>("兵数", 64, x => x.Quantity, 2),
                    Col<Military, int>("士气", 44, x => x.Morale, 2),
                    Col<Military, int>("战意", 44, x => x.Combativity, 2),
                    NumCol<Military>("途中", 72, x => x.Travel, 4),
                    Col<Military, string>("出发建筑", 80, x => x.StartingArchitectureString, 4),
                    Col<Military, string>("目标建筑", 80, x => x.TargetArchitectureString, 4)));
        }

        #endregion

        #region 兵种

        private static List<InfoView<MilitaryKind>> CreateMilitaryKindViews()
        {
            Func<MilitaryKind, string> name = x => x.Name;
            int nameWidth = MilitaryKindNameWidth;
            return Views(
                View("基本", name, nameWidth,
                    Col<MilitaryKind, MilitaryType>("种类", 64, x => x.Type),
                    Col<MilitaryKind, int>("单位规模", 72, x => x.MinScale, 3),
                    Col<MilitaryKind, int>("最大规模", 72, x => x.MaxScale, 3),
                    Col<MilitaryKind, int>("新编费用", 72, x => x.CreateCost, 2),
                    Col<MilitaryKind, int>("新编技术", 72, x => x.CreateTechnology, 3),
                    Col<MilitaryKind, int>("每兵所需技巧", 100, x => x.PointsPerSoldier, 3),
                    Col<MilitaryKind, string>("水边新编", 72, x => x.CreateBesideWaterString, 2)),
                View("战斗", name, nameWidth,
                    Col<MilitaryKind, int>("基础攻击力", 86, x => x.Offence, 3),
                    Col<MilitaryKind, int>("基础防御力", 86, x => x.Defence, 3),
                    Col<MilitaryKind, int>("攻击范围", 72, x => x.OffenceRadius, 4),
                    Col<MilitaryKind, string>("斜向攻击", 72, x => x.ObliqueOffenceString, 4),
                    Col<MilitaryKind, int>("影响个数", 72, x => x.InfluenceCount, 4)),
                View("单位", name, nameWidth,
                    Col<MilitaryKind, int>("单位规模", 72, x => x.MinScale, 3),
                    Col<MilitaryKind, int>("每单位攻击力", 100, x => x.OffencePerScale, 3),
                    Col<MilitaryKind, int>("每单位防御力", 100, x => x.DefencePerScale, 3),
                    Col<MilitaryKind, int>("每百经验攻击力", 110, x => x.OffencePer100Experience, 4),
                    Col<MilitaryKind, int>("每百经验防御力", 110, x => x.DefencePer100Experience, 4)),
                View("特色", name, nameWidth,
                    Col<MilitaryKind, float>("建筑伤害率", 86, x => x.ArchitectureDamageRate, 4),
                    Col<MilitaryKind, float>("建筑反击率", 86, x => x.ArchitectureCounterDamageRate, 4),
                    Col<MilitaryKind, int>("视野范围", 72, x => x.ViewRadius, 4),
                    Col<MilitaryKind, bool>("斜向视野", 72, x => x.ObliqueView, 4),
                    Col<MilitaryKind, float>("受火伤率", 72, x => x.FireDamageRate, 4),
                    Col<MilitaryKind, int>("编队上限", 72, x => x.RecruitLimit, 4)),
                View("适性", name, nameWidth,
                    Col<MilitaryKind, int>("行动力", 56, x => x.Movability, 4),
                    Col<MilitaryKind, int>("平原", 44, x => x.PlainAdaptability, 4),
                    Col<MilitaryKind, int>("草原", 44, x => x.GrasslandAdaptability, 4),
                    Col<MilitaryKind, int>("森林", 44, x => x.ForrestAdaptability, 4),
                    Col<MilitaryKind, int>("湿地", 44, x => x.MarshAdaptability, 4),
                    Col<MilitaryKind, int>("山地", 44, x => x.MountainAdaptability, 4),
                    Col<MilitaryKind, int>("水域", 44, x => x.WaterAdaptability, 4),
                    Col<MilitaryKind, int>("峻岭", 44, x => x.RidgeAdaptability, 4),
                    Col<MilitaryKind, int>("荒地", 44, x => x.WastelandAdaptability, 4),
                    Col<MilitaryKind, int>("栈道", 44, x => x.DesertAdaptability, 4),
                    Col<MilitaryKind, int>("瘴气林", 56, x => x.CliffAdaptability, 4)),
                View("乘数", name, nameWidth,
                    Col<MilitaryKind, float>("平原", 44, x => x.PlainRate, 4),
                    Col<MilitaryKind, float>("草原", 44, x => x.GrasslandRate, 4),
                    Col<MilitaryKind, float>("森林", 44, x => x.ForrestRate, 4),
                    Col<MilitaryKind, float>("湿地", 44, x => x.MarshRate, 4),
                    Col<MilitaryKind, float>("山地", 44, x => x.MountainRate, 4),
                    Col<MilitaryKind, float>("水域", 44, x => x.WaterRate, 4),
                    Col<MilitaryKind, float>("峻岭", 44, x => x.RidgeRate, 4),
                    Col<MilitaryKind, float>("荒地", 44, x => x.WastelandRate, 4),
                    Col<MilitaryKind, float>("栈道", 44, x => x.DesertRate, 4),
                    Col<MilitaryKind, float>("瘴气林", 56, x => x.CliffRate, 4)),
                View("介绍", name, nameWidth,
                    Col<MilitaryKind, string>("介绍", 0, x => x.Description, 3)));
        }

        #endregion

        #region 军区

        private static List<InfoView<Section>> CreateSectionViews()
        {
            Func<Section, string> name = x => x.Name;
            int nameWidth = SectionNameWidth;
            return Views(
                View("基本", name, nameWidth,
                    Col<Section, string>("所属势力", 80, x => x.FactionString),
                    Col<Section, string>("委任模式", 80, x => x.AIDetailString, 3),
                    Col<Section, string>("委任目标", 80, x => x.OrientationString, 3),
                    Col<Section, int>("人口总数", 86, x => x.Population, 3),
                    Col<Section, int>("军队总数", 80, x => x.Army, 3),
                    Col<Section, int>("资金总数", 86, x => x.Fund, 3),
                    Col<Section, int>("军粮总数", 100, x => x.Food, 3)),
                View("数量", name, nameWidth,
                    Col<Section, string>("所属势力", 80, x => x.FactionString),
                    Col<Section, int>("建筑数量", 72, x => x.ArchitectureCount, 3),
                    Col<Section, int>("人物数量", 72, x => x.PersonCount, 3),
                    Col<Section, int>("编队数量", 72, x => x.MilitaryCount, 3),
                    Col<Section, int>("部队数量", 72, x => x.TroopCount, 3)));
        }

        #endregion

        #region 宝物

        private static List<InfoView<Treasure>> CreateTreasureViews()
        {
            Func<Treasure, string> name = x => x.Name;
            int nameWidth = TreasureNameWidth;
            return Views(
                View("基本", name, nameWidth,
                    Col<Treasure, int>("价值", 56, x => x.Worth, 4),
                    Col<Treasure, string>("所属", 80, x => x.BelongedPersonString, 3),
                    Col<Treasure, int>("耐久", 56, x => x.Durability, 7),
                    Col<Treasure, string>("简介", 0, x => x.Description, 3)),
                View("效果", name, nameWidth,
                    Col<Treasure, string>("效果", 0, x => x.InfluenceString, 3)));
        }

        #endregion
    }
}
