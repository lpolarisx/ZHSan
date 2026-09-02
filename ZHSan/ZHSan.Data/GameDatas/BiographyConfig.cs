
namespace GameDatas;

public class BiographyConfig
{
    public int Id { get; set; }

    // <summary>
    /// 简要
    /// </summary>
    public string Brief { get; set; }

    /// <summary>
    /// 势力颜色 此武将自立时使用的势力颜色
    /// </summary>
    public int FactionColor { get; set; }

    /// <summary>
    /// 历史
    /// </summary>
    public string History { get; set; }

    /// <summary>
    /// 演义
    /// </summary>
    public string Romance { get; set; }

    /// <summary>
    /// 剧本
    /// </summary>
    public string InGame { get; set; }

    /// <summary>
    /// 兵种列表 此武将自立时使用的基本兵种
    /// </summary>
    public string MilitaryKindsString { get; set; }
}