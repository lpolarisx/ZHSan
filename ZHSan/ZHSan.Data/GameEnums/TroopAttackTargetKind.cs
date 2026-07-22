
using System.ComponentModel;

namespace GameEnums;

public enum TroopAttackTargetKind
{
    /// <summary>
    /// 遇敌
    /// </summary>
    [Description("遇敌则攻击")]
    EncounterEnemy = 0,

    /// <summary>
    /// 无反
    /// </summary>
    [Description("只攻击无反击的目标")]
    NoCounterattack = 1,

    /// <summary>
    /// 目标
    /// </summary>
    [Description("只攻击特定目标")]
    Target = 2,

    /// <summary>
    /// 攻弱
    /// </summary>
    [Description("只攻击攻击力较弱的目标")]
    AttackWeak = 3,

    /// <summary>
    /// 防弱
    /// </summary>
    [Description("只攻击防御力较弱的目标")]
    DefenseWeak = 4,

    /// <summary>
    /// 攻防皆弱
    /// </summary>
    [Description("只攻击攻防皆较弱的目标")]
    AttackAndDefenseWeak = 5,

    /// <summary>
    /// 无反默认
    /// </summary>
    [Description("优先攻击无反击的目标，若回合结束时未能达成攻击，则进行默认攻击")]
    NoCounterattackDefault = 6,

    /// <summary>
    /// 目标默认
    /// </summary>
    [Description("优先攻击特定目标，若回合结束时未能达成攻击，则进行默认攻击")]
    TargetDefault = 7,

    /// <summary>
    /// 攻弱默认
    /// </summary>
    [Description("优先攻击攻击力较弱的目标，若回合结束时未能达成攻击，则进行默认攻击")]
    AttackWeakDefault = 8,

    /// <summary>
    /// 防弱默认
    /// </summary>
    [Description("优先攻击防御力较弱的目标，若回合结束时未能达成攻击，则进行默认攻击")]
    DefenseWeakDefault = 9,

    /// <summary>
    /// 攻防皆弱默认
    /// </summary>
    [Description("优先攻击攻防皆较弱的目标，若回合结束时未能达成攻击，则进行默认攻击")]
    AttackAndDefenseWeakDefault = 10
}