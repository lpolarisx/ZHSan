
using GameEnums;

namespace GameDatas;

public class InfluenceKindConfig : BaseConfig
{
    /// <summary>
    /// 种类
    /// </summary>
    public InfluenceType Type { get; set; }

    /// <summary>
    /// 战斗
    /// </summary>
    public bool Combat { get; set; }

    /// <summary>
    /// 武将AI值
    /// </summary>
    public float AIPersonValue { get; set; }

    /// <summary>
    /// 武将AI值乘幂
    /// </summary>
    public float AIPersonValuePow { get; set; }

    /// <summary>
    /// 主将有效
    /// </summary>
    public bool TroopLeaderValid { get; set; }
}