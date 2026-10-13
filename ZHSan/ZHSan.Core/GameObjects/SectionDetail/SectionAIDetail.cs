using System.Runtime.Serialization;
using GameDatas;
using GameEnums;

namespace GameObjects.SectionDetail;

[DataContract]
public class SectionAIDetail : GameObject
{
    /// <summary>
    /// 说明
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// 目标种类
    /// </summary>
    [DataMember]
    public SectionOrientationKind OrientationKind { get; set; }

    /// <summary>
    /// 是否自动执行
    /// </summary>
    [DataMember]
    public bool AutoRun { get; set; }

    /// <summary>
    /// 重视农业
    /// </summary>
    [DataMember]
    public bool ValueAgriculture { get; set; }

    /// <summary>
    /// 重视商业
    /// </summary>
    [DataMember]
    public bool ValueCommerce { get; set; }

    /// <summary>
    /// 重视技术
    /// </summary>
    [DataMember]
    public bool ValueTechnology { get; set; }

    /// <summary>
    /// 重视统治
    /// </summary>
    [DataMember]
    public bool ValueDomination { get; set; }

    /// <summary>
    /// 重视民心
    /// </summary>
    [DataMember]
    public bool ValueMorale { get; set; }

    /// <summary>
    /// 重视耐久
    /// </summary>
    [DataMember]
    public bool ValueEndurance { get; set; }

    /// <summary>
    /// 重视训练
    /// </summary>
    [DataMember]
    public bool ValueTraining { get; set; }

    /// <summary>
    /// 重视补充
    /// </summary>
    [DataMember]
    public bool ValueRecruitment { get; set; }

    /// <summary>
    /// 重视新建编队
    /// </summary>
    [DataMember]
    public bool ValueNewMilitary { get; set; }

    /// <summary>
    /// 重视攻击
    /// </summary>
    [DataMember]
    public bool ValueOffensiveCampaign { get; set; }

    /// <summary>
    /// 允许使用情报和间谍
    /// </summary>
    [DataMember]
    public bool AllowInvestigateTactics { get; set; }

    /// <summary>
    /// 允许使用煽动和破坏
    /// </summary>
    [DataMember]
    public bool AllowOffensiveTactics { get; set; }

    /// <summary>
    /// 允许使用流言和说服
    /// </summary>
    [DataMember]
    public bool AllowPersonTactics { get; set; }

    /// <summary>
    /// 允许攻击
    /// </summary>
    [DataMember]
    public bool AllowOffensiveCampaign { get; set; }

    /// <summary>
    /// 允许输送资金
    /// </summary>
    [DataMember]
    public bool AllowFundTransfer { get; set; }

    /// <summary>
    /// 允许输送粮草
    /// </summary>
    [DataMember]
    public bool AllowFoodTransfer { get; set; }

    /// <summary>
    /// 允许输送部队
    /// </summary>
    [DataMember]
    public bool AllowMilitaryTransfer { get; set; }

    /// <summary>
    /// 允许拆除设施
    /// </summary>
    [DataMember]
    public bool AllowFacilityRemoval { get; set; }

    /// <summary>
    /// 允许新编编队
    /// </summary>
    [DataMember]
    public bool AllowNewMilitary { get; set; }

    public SectionAIDetail(SectionAIDetailConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        Description = config.Description;
        OrientationKind = config.OrientationKind;
        AutoRun = config.AutoRun;
        ValueAgriculture = config.ValueAgriculture;
        ValueCommerce = config.ValueCommerce;
        ValueDomination = config.ValueDomination;
        ValueMorale = config.ValueMorale;
        ValueEndurance = config.ValueEndurance;
        ValueTraining = config.ValueTraining;
        ValueRecruitment = config.ValueRecruitment;
        ValueNewMilitary = config.ValueNewMilitary;
        ValueOffensiveCampaign = config.ValueOffensiveCampaign;
        AllowInvestigateTactics = config.AllowInvestigateTactics;
        AllowOffensiveTactics = config.AllowOffensiveTactics;
        AllowPersonTactics = config.AllowPersonTactics;
        AllowOffensiveCampaign = config.AllowOffensiveCampaign;
        AllowFundTransfer = config.AllowFundTransfer;
        AllowFoodTransfer = config.AllowFoodTransfer;
        AllowMilitaryTransfer = config.AllowMilitaryTransfer;
        AllowFacilityRemoval = config.AllowFacilityRemoval;
        AllowNewMilitary = config.AllowNewMilitary;
    }

    public string OrientationKindString => OrientationKind.ToString();
}