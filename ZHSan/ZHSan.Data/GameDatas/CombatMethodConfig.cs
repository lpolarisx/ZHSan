
using GameEnums;

namespace GameDatas;

public class CombatMethodConfig : BaseConfig
{
    /// <summary>
    /// 所需战意
    /// </summary>
    public int Combativity { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 目标可能为建筑
    /// </summary>
    public bool ArchitectureTarget { get; set; }

    /// <summary>
    /// 使用条件
    /// </summary>
    public string CastConditionsString { get; set; }

    /// <summary>
    /// 视野内敌军越多越有可能使用
    /// </summary>
    public bool ViewingHostile { get; set; }

    /// <summary>
    /// 动画
    /// </summary>
    public TileAnimationKind AnimationKind { get; set; }

    /// <summary>
    /// 攻击默认类型
    /// </summary>
    public int AttackDefaultString { get; set; }

    /// <summary>
    /// 攻击目标类型
    /// </summary>
    public int AttackTargetString { get; set; }

    public string AIConditionWeightSelfString { get; set; }

    public string AIConditionWeightEnemyString { get; set; }
}