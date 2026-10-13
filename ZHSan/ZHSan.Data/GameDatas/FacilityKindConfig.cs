using GameEnums;

namespace GameDatas;

/// <summary>
/// 设施种类
/// </summary>
public class FacilityKindConfig : BaseConfig
{
    /// <summary>
    /// AI强度
    /// </summary>
    public float AILevel { get; set; }

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

    /// <summary>
    /// 不可拆除
    /// </summary>
    public bool IsDemolishable { get; set; }

    /// <summary>
    /// AI兴建条件权重
    /// </summary>
    public string AIBuildConditionWeightString { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public FacilityType Type { get; set; }
}
