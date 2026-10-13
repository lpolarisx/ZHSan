
using System.Collections.Generic;
using System.Linq;
using GameDatas;
using GameGlobal;
using GameManager;
using GameObjects.Conditions;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail;

public class FacilityKindLevel : GameObject
{
    public int Id { get; set; }

    /// <summary>
    /// 设施种类Id
    /// </summary>
    public int KindId { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// 占用位置
    /// </summary>
    public int PositionOccupied { get; set; }

    /// <summary>
    /// 新建所需技术
    /// </summary>
    public int TechnologyNeeded { get; set; }

    /// <summary>
    /// 新建所需技巧
    /// </summary>
    public int PointCost { get; set; }

    /// <summary>
    /// 新建所需资金
    /// </summary>
    public int FundCost { get; set; }

    /// <summary>
    /// 维持费用
    /// </summary>
    public int MaintenanceCost { get; set; }

    /// <summary>
    /// 建造所需时间
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// 耐久度
    /// </summary>
    public int Endurance { get; set; }

    /// <summary>
    /// 影响
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 兴建条件
    /// </summary>
    public string ConditionTableString { get; set; }

    /// <summary>
    /// 可容纳妃子数
    /// </summary>
    public int ConcubineCapacity { get; set; }

    public FacilityKindLevel(FacilityKindLevelConfig config)
    {
        Id = config.Id;
        KindId = config.KindId;
        Level = config.Level;
        PositionOccupied = config.PositionOccupied;
        TechnologyNeeded = config.TechnologyNeeded;
        PointCost = config.PointCost;
        FundCost = config.FundCost;
        MaintenanceCost = config.MaintenanceCost;
        Days = config.Days;
        Endurance = config.Endurance;
        InfluencesString = config.InfluencesString;
        ConditionTableString = config.ConditionTableString;
        ConcubineCapacity = ConcubineCapacity;
    }

    public new string Name => facilityKind.Name;

    public float AILevel => facilityKind.AILevel;

    public int ArchitectureLimit => facilityKind.ArchitectureLimit;

    public int FactionLimit => facilityKind.FactionLimit;

    public bool IsDemolishable => facilityKind.IsDemolishable;

    public int DaysText => Days * Session.Parameters.DayInTurn;

    public string Description
    {
        get
        {
            var str = ConcubineCapacity > 0 ? $"•可以容纳{ConcubineCapacity}名美女" : "";

            str += string.Join("•", Influences.Select(x => x.Description));

            return str;
        }
    }

    public string ConditionString => StaticMethods.SaveNameToString(Conditions);

    public Dictionary<Condition, float> AIBuildConditionWeight => facilityKind.AIBuildConditionWeight;

    public List<Influence> Influences { get; set; } = new();

    public List<Condition> Conditions { get; set; } = new();

    private FacilityKind facilityKind => Session.Current.Scenario.GameCommonData.AllFacilityKinds.GetValueOrDefault(KindId);

    /// <summary>
    /// 获取AI值
    /// </summary>
    /// <param name="architecture"></param>
    /// <returns></returns>
    public float AiValue(Architecture architecture)
    {
        // TODO:影响为空时返回一个很小的负数，是否调整为0
        var value = Influences.Any() ? Influences.Max(x => x.AIFacilityValue(architecture)) : double.MinValue;

        if (value >= 0)
        {
            value = (value - MaintenanceCost / architecture.ExpectedFund * 30.0) * facilityKind.AILevel / PositionOccupied;
        }

        return (float)value;
    }

    /// <summary>
    /// 是否盈利
    /// </summary>
    public bool IsProfitable
    {
        get
        {
            int fundIncrease = 0;
            foreach (var influence in Influences)
            {
                var influenceKindId = influence.Kind.ID;

                if (influenceKindId == 3000)
                {
                    fundIncrease += influence.GetIntParam();
                }
                else if (influenceKindId == 3020 || influenceKindId == 3210)
                {
                    // 资金加成 & 商业值增长默认盈利
                    return true;
                }
            }

            // 资金增长扣除成本
            var isProfitable = fundIncrease - MaintenanceCost * 30 > 0;

            return isProfitable;
        }
    }

    /// <summary>
    /// 是否可增筑
    /// </summary>
    public bool IsExtension
    {
        get
        {
            // TODO: 用占用位置和影响种类判断增筑，是否用增筑Id直接判断

            // 增筑 & 治所占用位置为0
            if (PositionOccupied > 0) return false;

            var influenceKindIds = Influences.Select(x => x.Kind.ID);

            // 农业、商业、技术、耐久、设施空间、人口上限
            var targetKindIds = new[] { 1000, 1001, 1002, 1003, 1020, 1050 };

            var isExtension = influenceKindIds.Any(x => targetKindIds.Contains(x));

            return isExtension;
        }
    }

    /// <summary>
    /// 可建造
    /// </summary>
    /// <param name="architecture">建筑</param>
    /// <returns></returns>
    public bool CanBuild(Architecture architecture)
    {
        // 9999表示无上限
        var noLimit = 9999;

        // 建筑总空间不足
        if (PositionOccupied > 0 && architecture.FacilityPositionCount == 0)
            return false;

        // 设施人口相关不匹配
        if (facilityKind.PopulationRelated && !architecture.Kind.HasPopulation)
            return false;

        // 建筑技术不足
        if (TechnologyNeeded > architecture.Technology)
            return false;

        if (!Condition.CheckConditionList(Conditions, architecture))
            return false;

        // 已达建筑上限
        if (facilityKind.ArchitectureLimit < noLimit && facilityKind.ArchitectureLimit <= architecture.GetFacilityKindCount(KindId))
            return false;


        var faction = architecture.BelongedFaction;

        // 势力技巧不足
        var factionPoint = faction != null ? faction.TechniquePoint + faction.TechniquePointForFacility + faction.TechniquePointForTechnique : 0;
        if (PointCost > factionPoint)
            return false;

        // 已达势力上限
        var factionLimit = faction?.GetFacilityKindCount(KindId) ?? 0;
        if (facilityKind.FactionLimit < noLimit && facilityKind.FactionLimit <= factionLimit)
            return false;

        return true;
    }
}