
namespace GameDatas;

public class CaptiveConfig
{
    public int Id { get; set; }

    /// <summary>
    /// 俘虏势力
    /// </summary>
    public int CaptiveFactionId { get; set; }

    /// <summary>
    /// 俘虏人物
    /// </summary>
    public int CaptivePersonId { get; set; }

    /// <summary>
    /// 赎金目标建筑
    /// </summary>
    public int RansomArchitectureId { get; set; }

    /// <summary>
    /// 赎金到达时间
    /// </summary>
    public int RansomArriveDays { get; set; }

    /// <summary>
    /// 赎金金额
    /// </summary>
    public int RansomFund { get; set; }
}