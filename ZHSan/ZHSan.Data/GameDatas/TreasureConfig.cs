
namespace GameDatas;

public class TreasureConfig : BaseConfig
{
    /// <summary>
    /// 图像
    /// </summary>
    public int Pic { get; set; }

    /// <summary>
    /// 价值
    /// </summary>
    public int Worth { get; set; }

    /// <summary>
    /// 已出现
    /// </summary>
    public bool Available { get; set; }

    /// <summary>
    /// 隐藏于建筑
    /// </summary>
    public int HidePlaceIDString { get; set; }

    /// <summary>
    /// 宝物种类：此值相同的话，这些宝物不叠加
    /// </summary>
    public int TreasureGroup { get; set; }

    /// <summary>
    /// 出现年
    /// </summary>
    public int AppearYear { get; set; }

    /// <summary>
    /// 属于人物
    /// </summary>
    public int BelongedPersonIDString { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 介绍
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 耐久
    /// </summary>
    public int Durability { get; set; }
}