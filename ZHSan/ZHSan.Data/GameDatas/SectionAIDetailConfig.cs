
using GameEnums;

namespace GameDatas;

public class SectionAIDetailConfig : BaseConfig
{
    /// <summary>
    /// 说明
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 目标种类
    /// </summary>
    public SectionOrientationKind OrientationKind { get; set; }

    /// <summary>
    /// 是否自动执行
    /// </summary>
    public bool AutoRun { get; set; }

    /// <summary>
    /// 重视农业
    /// </summary>
    public bool ValueAgriculture { get; set; }

    /// <summary>
    /// 重视商业
    /// </summary>
    public bool ValueCommerce { get; set; }

    /// <summary>
    /// 重视技术
    /// </summary>
    public bool ValueTechnology { get; set; }

    /// <summary>
    /// 重视统治
    /// </summary>
    public bool ValueDomination { get; set; }

    /// <summary>
    /// 重视民心
    /// </summary>
    public bool ValueMorale { get; set; }

    /// <summary>
    /// 重视耐久
    /// </summary>
    public bool ValueEndurance { get; set; }

    /// <summary>
    /// 重视训练
    /// </summary>
    public bool ValueTraining { get; set; }

    /// <summary>
    /// 重视补充
    /// </summary>
    public bool ValueRecruitment { get; set; }

    /// <summary>
    /// 重视新建编队
    /// </summary>
    public bool ValueNewMilitary { get; set; }

    /// <summary>
    /// 重视攻击
    /// </summary>
    public bool ValueOffensiveCampaign { get; set; }

    /// <summary>
    /// 允许使用情报和间谍
    /// </summary>
    public bool AllowInvestigateTactics { get; set; }

    /// <summary>
    /// 允许使用煽动和破坏
    /// </summary>
    public bool AllowOffensiveTactics { get; set; }

    /// <summary>
    /// 允许使用流言和说服
    /// </summary>
    public bool AllowPersonTactics { get; set; }

    /// <summary>
    /// 允许攻击
    /// </summary>
    public bool AllowOffensiveCampaign { get; set; }

    /// <summary>
    /// 允许输送资金
    /// </summary>
    public bool AllowFundTransfer { get; set; }

    /// <summary>
    /// 允许输送粮草
    /// </summary>
    public bool AllowFoodTransfer { get; set; }

    /// <summary>
    /// 允许输送部队
    /// </summary>
    public bool AllowMilitaryTransfer { get; set; }

    /// <summary>
    /// 允许拆除设施
    /// </summary>
    public bool AllowFacilityRemoval { get; set; }

    /// <summary>
    /// 允许新编编队
    /// </summary>
    public bool AllowNewMilitary { get; set; }
}