
using GameEnums;

namespace GameDatas;

public class InformationKindConfig : BaseConfig
{
    /// <summary>
    /// 消耗资金
    /// </summary>
    public int CostFund { get; set; }
    
    /// <summary>
    /// 等级
    /// </summary>
    public InformationLevel Level { get; set; }

    /// <summary>
    /// 斜向
    /// </summary>
    public bool Oblique { get; set; }

    /// <summary>
    /// 半径范围
    /// </summary>
    public int Radius { get; set; }
}