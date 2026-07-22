
using System.ComponentModel;

namespace GameEnums;

/// <summary>
/// 部队施法默认类型
/// </summary>
public enum TroopCastDefaultKind
{
    /// <summary>
    /// 智最弱
    /// </summary>
    [Description("以计略范围内智谋最低的部队为目标")]
    IntelligenceWeakest = 0,

    /// <summary>
    /// 智最强
    /// </summary>
    [Description("以计略范围内智谋最高的部队为目标")]
    IntelligenceStrongest = 1,

    /// <summary>
    /// 士最低
    /// </summary>
    [Description("以计略范围内士气最低的部队为目标")]
    MoraleLowest = 2,

    /// <summary>
    /// 士最高
    /// </summary>
    [Description("以计略范围内士气最高的部队为目标")]
    MoraleHighest = 3
}