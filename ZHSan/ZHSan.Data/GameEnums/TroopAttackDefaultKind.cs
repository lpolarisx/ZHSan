
using System.ComponentModel;

namespace GameEnums;

public enum TroopAttackDefaultKind
{
    /// <summary>
    /// 防最弱
    /// </summary>
    [Description("攻击部队优先，以攻击范围内防御力最弱的敌军为对象")]
    WeakestDefense = 0,

    /// <summary>
    /// 防最强
    /// </summary>
    [Description("攻击部队优先，以攻击范围内防御力最强的敌军为对象")]
    StrongestDefense = 1,

    /// <summary>
    /// 攻最弱
    /// </summary>
    [Description("攻击部队优先，以攻击范围内攻击力最弱的敌军为对象")]
    WeakestAttack = 2,

    /// <summary>
    /// 攻最强
    /// </summary>
    [Description("攻击部队优先，以攻击范围内攻击力最强的敌军为对象")]
    StrongestAttack = 3,

    /// <summary>
    /// 耐最低
    /// </summary>
    [Description("攻击建筑优先，以攻击范围内耐久值最低的建筑为对象")]
    LowestEndurance = 4,

    /// <summary>
    /// 耐最高
    /// </summary>
    [Description("攻击建筑优先，以攻击范围内耐久值最高的建筑为对象")]
    HighestEndurance = 5,

    /// <summary>
    /// 抗暴最低
    /// </summary>
    [Description("攻击部队优先，以攻击范围内抗暴几率最低的敌军为对象")]
    LowestCritResistance = 6,

    /// <summary>
    /// 抗暴最高
    /// </summary>
    [Description("攻击部队优先，以攻击范围内抗暴几率最高的敌军为对象")]
    HighestCritResistance = 7
}