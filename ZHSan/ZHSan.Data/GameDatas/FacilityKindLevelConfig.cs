namespace GameDatas;

/// <summary>
/// 设施种类等级
/// </summary>
public class FacilityKindLevelConfig
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
}
