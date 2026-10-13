
namespace GameDatas;

public class TerrainDetailConfig : BaseConfig
{
    /// <summary>
    /// 图形层次
    /// </summary>
    public int GraphicLayer { get; set; }

    /// <summary>
    /// 视线可穿透
    /// </summary>
    public bool ViewThrough { get; set; }

    /// <summary>
    /// 粮道开通资金消耗
    /// </summary>
    public int RoutewayBuildFundCost { get; set; }

    /// <summary>
    /// 粮道维持资金消耗
    /// </summary>
    public int RoutewayActiveFundCost { get; set; }

    /// <summary>
    /// 粮道开通工作量
    /// </summary>
    public int RoutewayBuildWorkCost { get; set; }

    /// <summary>
    /// 粮草消耗率
    /// </summary>
    public float RoutewayConsumptionRate { get; set; }

    /// <summary>
    /// 粮草蕴藏量
    /// </summary>
    public int FoodDeposit { get; set; }

    /// <summary>
    /// 粮草恢复天数
    /// </summary>
    public int FoodRegainDays { get; set; }

    /// <summary>
    /// 春粮系数
    /// </summary>
    public float FoodSpringRate { get; set; }

    /// <summary>
    /// 夏粮系数
    /// </summary>
    public float FoodSummerRate { get; set; }

    /// <summary>
    /// 秋粮系数
    /// </summary>
    public float FoodAutumnRate { get; set; }

    /// <summary>
    /// 冬粮系数
    /// </summary>
    public float FoodWinterRate { get; set; }

    /// <summary>
    /// 火焰伤害率
    /// </summary>
    public float FireDamageRate { get; set; }

    public bool CanExtendInto { get; set; }
}