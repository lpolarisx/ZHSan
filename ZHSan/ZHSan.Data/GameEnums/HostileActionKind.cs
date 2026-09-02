
namespace GameEnums;

public enum HostileActionKind
{
    /// <summary>
    /// 不理会
    /// </summary>
    Ignore = 0,

    /// <summary>
    /// 攻击
    /// </summary>
    Attack,

    /// <summary>
    /// 躲避影响范围
    /// </summary>
    EvadeEffect,

    /// <summary>
    /// 躲避视野范围
    /// </summary>
    EvadeView
}