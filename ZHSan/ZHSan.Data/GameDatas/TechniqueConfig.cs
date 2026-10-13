
namespace GameDatas;

public class TechniqueConfig : BaseConfig
{
    /// <summary>
    /// 种类
    /// </summary>
    public int Kind { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 升级时间
    /// </summary>
    public int Days { get; set; }

    /// <summary>
    /// 资金消耗
    /// </summary>
    public int FundCost { get; set; }

    /// <summary>
    /// 技巧点数消耗
    /// </summary>
    public int PointCost { get; set; }

    /// <summary>
    /// 需要声望
    /// </summary>
    public int Reputation { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 前置所需科技ID
    /// </summary>
    public int PreID { get; set; }

    /// <summary>
    /// 后置可学科技ID
    /// </summary>
    public int PostID { get; set; }

    /// <summary>
    /// 显示列
    /// </summary>
    public int DisplayCol { get; set; }

    /// <summary>
    /// 显示行
    /// </summary>
    public int DisplayRow { get; set; }

    /// <summary>
    /// AI条件列表
    /// </summary>
    public string AIConditionWeightString { get; set; }

    /// <summary>
    /// 条件列表
    /// </summary>
    public string ConditionTableString { get; set; }
}