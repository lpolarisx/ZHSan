
using GameEnums;

namespace GameDatas;

public class StratagemConfig : BaseConfig
{
    /// <summary>
    /// 消耗战意
    /// </summary>
    public int Combativity { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 动画
    /// </summary>
    public TileAnimationKind AnimationKind { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 使用条件列表
    /// </summary>
    public string CastConditionsString { get; set; }

    public string AIConditionWeightSelfString { get; set; }

    public string AIConditionWeightEnemyString { get; set; }

    public bool ArchitectureTarget { get; set; }

    public int CastDefaultString { get; set; }

    public int CastTargetString { get; set; }

    public int Chance { get; set; }

    public bool Friendly { get; set; }

    public bool Self { get; set; }

    public int TechniquePoint { get; set; }

    public bool RequireInfluenceToUse { get; set; }
}