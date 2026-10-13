
using System.ComponentModel;

namespace GameEnums;

public enum TroopCastTargetKind
{
    /// <summary>
    /// 可能
    /// </summary>
    [Description("遇到可能目标则施展")]
    Possible = 0,
    
    /// <summary>
    /// 特定
    /// </summary>
    [Description("只对特定目标施展")]
    Specific = 1,

    /// <summary>
    /// 智低
    /// </summary>
    [Description("只对智力较低的目标施展")]
    IntelligenceLow = 2,

    /// <summary>
    /// 特定默认
    /// </summary>
    [Description("优先对特定目标施展，若回合结束时未能达成施展，则进行默认施展")]
    SpecificDefault = 3,

    /// <summary>
    /// 智低默认
    /// </summary>
    [Description("优先对智力较低的目标施展，若回合结束时未能达成施展，则进行默认施展")]
    IntelligenceLowDefault = 4
}