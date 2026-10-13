
using System.Collections.Generic;
using GameEnums;

namespace GameDatas;

public class MilitaryKindConfig : BaseConfig
{
    /// <summary>
    /// 类别
    /// </summary>
    public MilitaryType Type { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 强度（AI）
    /// </summary>
    public int Merit { get; set; }

    /// <summary>
    /// 较强兵种ID：如果AI准备征召这个兵种的话，会考虑征召在这列表中的兵种，而这列表的兵种是绝对强于这个兵种
    /// </summary>
    public string SuccessorString { get; set; }

    /// <summary>
    /// 行动速率：行动速率高的部队将优先行动。行动速率＝兵种本身的行动速率×士气÷士气上限
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// 获得机率：每天有1除以此数的机率，拥有武将的势力可获得这个兵种
    /// </summary>
    public int ObtainProb { get; set; }

    /// <summary>
    /// 出兵称号影响
    /// </summary>
    public int TitleInfluence { get; set; } = -1;

    /// <summary>
    /// 新建资金
    /// </summary>
    public int CreateCost { get; set; }

    /// <summary>
    /// 新建所需技术
    /// </summary>
    public int CreateTechnology { get; set; }

    /// <summary>
    /// 水边新建
    /// </summary>
    public bool CreateBesideWater { get; set; }

    /// <summary>
    /// 攻击
    /// </summary>
    public int Offence { get; set; }

    /// <summary>
    /// 防御
    /// </summary>
    public int Defence { get; set; }

    /// <summary>
    /// 攻击半径
    /// </summary>
    public int OffenceRadius { get; set; }

    /// <summary>
    /// 能否反击
    /// </summary>
    public bool CounterOffence { get; set; }

    /// <summary>
    /// 能否被反击
    /// </summary>
    public bool BeCountered { get; set; }

    /// <summary>
    /// 斜向攻击
    /// </summary>
    public bool ObliqueOffence { get; set; }

    /// <summary>
    /// 箭矢攻击：弓箭攻击，投石车等部队不属于弓箭攻击
    /// </summary>
    public bool ArrowOffence { get; set; }

    /// <summary>
    /// 凌空攻击：是否可以攻击建筑内的部队
    /// </summary>
    public bool AirOffence { get; set; }

    /// <summary>
    /// 近身攻击
    /// </summary>
    public bool ContactOffence { get; set; }

    /// <summary>
    /// 建筑伤害系数
    /// </summary>
    public float ArchitectureDamageRate { get; set; }

    /// <summary>
    /// 建筑反击承受率
    /// </summary>
    public float ArchitectureCounterDamageRate { get; set; }

    /// <summary>
    /// 计略范围
    /// </summary>
    public int StratagemRadius { get; set; }

    /// <summary>
    /// 斜向计略
    /// </summary>
    public bool ObliqueStratagem { get; set; }

    /// <summary>
    /// 视野半径
    /// </summary>
    public int ViewRadius { get; set; }

    /// <summary>
    /// 斜向视野
    /// </summary>
    public bool ObliqueView { get; set; }

    /// <summary>
    /// 伤兵概率
    /// </summary>
    public int InjuryChance { get; set; }

    /// <summary>
    /// 行动力
    /// </summary>
    public int Movability { get; set; }

    /// <summary>
    /// 单一适性种类
    /// </summary>
    public int OneAdaptabilityKind { get; set; }

    /// <summary>
    /// 平原适性
    /// </summary>
    public int PlainAdaptability { get; set; }

    /// <summary>
    /// 草地适性
    /// </summary>
    public int GrasslandAdaptability { get; set; }

    /// <summary>
    /// 森林适性
    /// </summary>
    public int ForrestAdaptability { get; set; }

    /// <summary>
    /// 湿地适性
    /// </summary>
    public int MarshAdaptability { get; set; }

    /// <summary>
    /// 山地适性
    /// </summary>
    public int MountainAdaptability { get; set; }

    /// <summary>
    /// 水域适性
    /// </summary>
    public int WaterAdaptability { get; set; }

    /// <summary>
    /// 峻岭适性
    /// </summary>
    public int RidgeAdaptability { get; set; }

    /// <summary>
    /// 荒地适性
    /// </summary>
    public int WastelandAdaptability { get; set; }

    /// <summary>
    /// 沙漠适性
    /// </summary>
    public int DesertAdaptability { get; set; }

    /// <summary>
    /// 棧道适性
    /// </summary>
    public int CliffAdaptability { get; set; }

    /// <summary>
    /// 平原乘数
    /// </summary>
    public float PlainRate { get; set; }

    /// <summary>
    /// 草地乘数
    /// </summary>
    public float GrasslandRate { get; set; }

    /// <summary>
    /// 森林乘数
    /// </summary>
    public float ForrestRate { get; set; }

    /// <summary>
    /// 湿地乘数
    /// </summary>
    public float MarshRate { get; set; }

    /// <summary>
    /// 山地乘数
    /// </summary>
    public float MountainRate { get; set; }

    /// <summary>
    /// 水域乘数
    /// </summary>
    public float WaterRate { get; set; }

    /// <summary>
    /// 峻岭乘数
    /// </summary>
    public float RidgeRate { get; set; }

    /// <summary>
    /// 荒地乘数
    /// </summary>
    public float WastelandRate { get; set; }

    /// <summary>
    /// 沙漠乘数
    /// </summary>
    public float DesertRate { get; set; }

    /// <summary>
    /// 棧道乘数
    /// </summary>
    public float CliffRate { get; set; }

    /// <summary>
    /// 受火伤率
    /// </summary>
    public float FireDamageRate { get; set; }

    /// <summary>
    /// 势力编队上限
    /// </summary>
    public int RecruitLimit { get; set; }

    /// <summary>
    /// 每个士兵每天消耗的粮草数
    /// </summary>
    public int FoodPerSoldier { get; set; }

    /// <summary>
    /// 口粮天数
    /// </summary>
    public int RationDays { get; set; }

    /// <summary>
    /// 每补充1人所需的技巧点数
    /// </summary>
    public int PointsPerSoldier { get; set; }

    /// <summary>
    /// 成军最小规模
    /// </summary>
    public int MinScale { get; set; }

    /// <summary>
    /// 一个单位规模所增加的攻击力
    /// </summary>
    public int OffencePerScale { get; set; }

    /// <summary>
    /// 一个单位规模所增加的防御力
    /// </summary>
    public int DefencePerScale { get; set; }

    /// <summary>
    /// 最大规模
    /// </summary>
    public int MaxScale { get; set; }

    /// <summary>
    /// 能否升级
    /// </summary>
    public bool CanLevelUp { get; set; }

    /// <summary>
    /// 升级成的兵种ID
    /// </summary>
    public List<int> LevelUpKindID { get; set; } = new();

    /// <summary>
    /// 升级经验
    /// </summary>
    public int LevelUpExperience { get; set; }

    /// <summary>
    /// 每一百经验增加的攻击力
    /// </summary>
    public int OffencePer100Experience { get; set; }

    /// <summary>
    /// 每一百经验增加的防御力
    /// </summary>
    public int DefencePer100Experience { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 最低统率(AI)
    /// </summary>
    public int MinCommand { get; set; }

    /// <summary>
    /// 新编条件：编队所在建筑条件
    /// </summary>
    public string CreateConditionsString { get; set; }

    /// <summary>
    /// 资金上限
    /// </summary>
    public int zijinshangxian { get; set; }

    /// <summary>
    /// 攻击默认类型
    /// </summary>
    public TroopAttackDefaultKind AttackDefaultKind { get; set; }

    /// <summary>
    /// 攻击目标类型
    /// </summary>
    public TroopAttackTargetKind AttackTargetKind { get; set; }

    /// <summary>
    /// 施展默认类型
    /// </summary>
    public TroopCastDefaultKind CastDefaultKind { get; set; }

    /// <summary>
    /// 施展目标类型
    /// </summary>
    public TroopCastTargetKind CastTargetKind { get; set; }

    /// <summary>
    /// 是否外壳
    /// </summary>
    public bool IsShell { get; set; }

    /// <summary>
    /// 只能在移动前攻击
    /// </summary>
    public bool OffenceOnlyBeforeMove { get; set; }

    /// <summary>
    /// 变换至兵种
    /// </summary>
    public int MorphToKindId { get; set; }

    public string AICreateArchitectureConditionWeightString { get; set; }

    public string AIUpgradeArchitectureConditionWeightString { get; set; }

    public string AIUpgradeLeaderConditionWeightString { get; set; }

    public string AILeaderConditionWeightString { get; set; }
}