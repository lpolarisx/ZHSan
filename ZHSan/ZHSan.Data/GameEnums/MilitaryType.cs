
using System.ComponentModel;

namespace GameEnums;

public enum MilitaryType
{
    /// <summary>
    /// 步兵
    /// </summary>
    [Description("步兵")]
    Infantry = 0,

    /// <summary>
    /// 弩兵
    /// </summary>
    [Description("弩兵")]
    Crossbow = 1,

    /// <summary>
    /// 骑兵
    /// </summary>
    [Description("骑兵")]
    Cavalry = 2,

    /// <summary>
    /// 水军
    /// </summary>
    [Description("水军")]
    Navy = 3,

    /// <summary>
    /// 器械
    /// </summary>
    [Description("器械")]
    SiegeEquipment = 4,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 5
}