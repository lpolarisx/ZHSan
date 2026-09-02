
namespace GameDatas;

public class MilitaryConfig : BaseConfig
{
    /// <summary>
    /// 种类ID
    /// </summary>
    public int KindId { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 士气
    /// </summary>
    public int Morale { get; set;}

    /// <summary>
    /// 战意
    /// </summary>
    public int Combativity { get; set; }

    /// <summary>
    /// 经验
    /// </summary>
    public int Experience { get; set; }

    /// <summary>
    /// 伤兵
    /// </summary>
    public int InjuryQuantity { get; set; }

    /// <summary>
    /// 追随将领ID
    /// </summary>
    public int FollowedLeaderID { get; set; }

    /// <summary>
    /// 队长ID
    /// </summary>
    public int LeaderID { get; set; }

    /// <summary>
    /// 队长经验
    /// </summary>
    public int LeaderExperience { get; set; }

    /// <summary>
    /// 疲累度
    /// </summary>
    public int Tiredness { get; set; }

    /// <summary>
    /// 到达时间
    /// </summary>
    public int ArrivingDays { get; set; }

    /// <summary>
    /// 所属建筑
    /// </summary>
    public int BelongedArchitectureID { get; set; }

    /// <summary>
    /// 出发建筑
    /// </summary>
    public int StartingArchitectureID { get; set; }

    /// <summary>
    /// 目标建筑
    /// </summary>
    public int TargetArchitectureID { get; set; }

    /// <summary>
    /// 被包裹编队
    /// </summary>
    public int ShelledMilitaryID { get; set; }

    /// <summary>
    /// 补充人员ID
    /// </summary>
    public int RecruitmentPersonID { get; set; }

    /// <summary>
    /// 总士兵伤害
    /// </summary>
    public int TroopDamageDealt { get; set; }

    /// <summary>
    /// 击破数
    /// </summary>
    public int RoutCount { get; set; }

    /// <summary>
    /// 编成年
    /// </summary>
    public int YearCreated { get; set; }
    
    /// <summary>
    /// 总受士兵伤害
    /// </summary>
    public int TroopBeDamageDealt { get; set; }

    /// <summary>
    /// 总建筑伤害
    /// </summary>
    public int ArchitectureDamageDealt { get; set; }

    /// <summary>
    /// 计略成功次数
    /// </summary>
    public int StratagemSuccessCount { get; set; }

    /// <summary>
    /// 计略失败次数
    /// </summary>
    public int StratagemFailCount { get; set; }

    /// <summary>
    /// 中计次数
    /// </summary>
    public int StratagemBeSuccessCount { get; set; }

    /// <summary>
    /// 计略阻挡次数
    /// </summary>
    public int StratagemBeFailCount { get; set; }

    /// <summary>
    /// 致武将战死数
    /// </summary>
    public int OfficerKillCount { get; set; }

    /// <summary>
    /// 俘获将领次数
    /// </summary>
    public int CaptiveCount { get; set; }
}