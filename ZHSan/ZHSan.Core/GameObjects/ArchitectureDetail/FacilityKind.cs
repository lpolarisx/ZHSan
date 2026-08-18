using GameObjects.Conditions;
using System.Collections.Generic;
using GameDatas;
using GameEnums;
using GameGlobal;

namespace GameObjects.ArchitectureDetail;
/// <summary>
/// 设施种类
/// </summary>
public class FacilityKind : GameObject
{
    #region DataMember

    /// <summary>
    /// AI强度
    /// </summary>
    public float AILevel { get; set; }

    // /// <summary>
    // /// 占用位置
    // /// </summary>
    // [DataMember]
    // public int PositionOccupied { get; set; }

    // /// <summary>
    // /// 新建所需技术
    // /// </summary>
    // [DataMember]
    // public int TechnologyNeeded { get; set; }

    // /// <summary>
    // /// 新建所需技巧
    // /// </summary>
    // [DataMember]
    // public int PointCost { get; set; }

    // /// <summary>
    // /// 新建所需资金
    // /// </summary>
    // [DataMember]
    // public int FundCost { get; set; }

    // /// <summary>
    // /// 维持费用
    // /// </summary>
    // [DataMember]
    // public int MaintenanceCost { get; set; }

    // /// <summary>
    // /// 建造所需时间
    // /// </summary>
    // [DataMember]
    // public int Days { get; set; }

    // /// <summary>
    // /// 耐久度
    // /// </summary>
    // [DataMember]
    // public int Endurance { get; set; }

    /// <summary>
    /// 建筑上限
    /// </summary>
    public int ArchitectureLimit { get; set; }

    /// <summary>
    /// 势力上限
    /// </summary>
    public int FactionLimit { get; set; }

    /// <summary>
    /// 人口相关
    /// </summary>
    public bool PopulationRelated { get; set; }

    // /// <summary>
    // /// 影响
    // /// </summary>
    // [DataMember]
    // public string InfluencesString { get; set; }

    // /// <summary>
    // /// 兴建条件
    // /// </summary>
    // [DataMember]
    // public string ConditionTableString { get; set; }

    // /// <summary>
    // /// 可容纳妃子数
    // /// </summary>
    // [DataMember]
    // public int rongna { get; set; }

    /// <summary>
    /// 不可拆除
    /// </summary>
    public bool IsDemolishable { get; set; }

    /// <summary>
    /// AI兴建条件权重
    /// </summary>
    public string AIBuildConditionWeightString { get; set; }

    public FacilityType Type { get; set; }

    #endregion

    public FacilityKind(FacilityKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        AILevel = config.AILevel;
        ArchitectureLimit = config.ArchitectureLimit;
        FactionLimit = config.FactionLimit;
        PopulationRelated = config.PopulationRelated;
        IsDemolishable = config.IsDemolishable;
        AIBuildConditionWeightString = config.AIBuildConditionWeightString;
        Type = config.Type;
    }

    public Dictionary<Condition, float> AIBuildConditionWeight { get; set; } = new();

    public string PopulationRelatedString => StaticMethods.ToMark(PopulationRelated);
}