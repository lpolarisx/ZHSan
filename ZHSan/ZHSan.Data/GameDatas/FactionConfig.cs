
using System.Collections.Generic;

namespace GameDatas;

public class FactionConfig : BaseConfig
{
    /// <summary>
    /// 君主ID
    /// </summary>
    public int LeaderID { get; set; }

    /// <summary>
    /// 颜色编号
    /// </summary>
    public int ColorIndex { get; set; }

    /// <summary>
    /// 都城ID
    /// </summary>
    public int CapitalID { get; set; }

    /// <summary>
    /// 技巧点数
    /// </summary>
    public int TechniquePoint { get; set; }

    /// <summary>
    /// 为升级技巧所保留的技巧点数
    /// </summary>
    public int TechniquePointForTechnique { get; set; }
    
    /// <summary>
    /// 为建造设施所保留的技巧点数
    /// </summary>
    public int TechniquePointForFacility { get; set; }

    /// <summary>
    /// 声望
    /// </summary>
    public int Reputation { get; set; }

    /// <summary>
    /// 军区列表
    /// </summary>
    public string SectionsString { get; set; }

    /// <summary>
    /// 情报列表
    /// </summary>
    public string InformationsString { get; set; }

    /// <summary>
    /// 建筑列表
    /// </summary>
    public string ArchitecturesString { get; set; }

    /// <summary>
    /// 部队列表
    /// </summary>
    public string TroopListString { get; set; }

    /// <summary>
    /// 粮道列表
    /// </summary>
    public string RoutewaysString { get; set; }

    /// <summary>
    /// 军团列表
    /// </summary>
    public string LegionsString { get; set; }

    /// <summary>
    /// 基本兵种列表
    /// </summary>
    public string BaseMilitaryKindsString { get; set; }

    /// <summary>
    /// 正在升级中的技巧
    /// </summary>
    public int UpgradingTechnique { get; set; }

    /// <summary>
    /// 正在升级中的技巧剩余时间
    /// </summary>
    public int UpgradingDaysLeft { get; set; }

    /// <summary>
    /// 已有技巧
    /// </summary>
    public string AvailableTechniquesString { get; set; }

    /// <summary>
    /// 偏好技巧类别
    /// </summary>
    public List<int> PreferredTechniqueKinds { get; set; } = new();

    /// <summary>
    /// 计划技巧
    /// </summary>
    public int PlanTechniqueString { get; set; }

    /// <summary>
    /// 自动拒绝释放俘虏
    /// </summary>
    public bool AutoRefuse { get; set; }

    /// <summary>
    /// 朝廷贡献度
    /// </summary>
    public int CourtContribution { get; set; }

    /// <summary>
    /// 官爵
    /// </summary>
    public int OfficialRank { get; set; }

    /// <summary>
    /// 异族
    /// </summary>
    public bool IsAlien { get; set; }

    /// <summary>
    /// 玩家不可选
    /// </summary>
    public bool NotPlayerSelectable { get; set; }

    /// <summary>
    /// 储君ID
    /// </summary>
    public int PrinceID { get; set; }

    /// <summary>
    /// 本年已招贤数量
    /// </summary>
    public int YearOfficialLimit { get; set; }

    /// <summary>
    /// 生成武将总数
    /// </summary>
    public string GetGeneratorPersonCountString { get; set; }
    
    /// <summary>
    /// 运输编队列表
    /// </summary>
    public string TransferingMilitariesString { get; set; }

    /// <summary>
    /// 编队列表
    /// </summary>
    public string MilitariesString { get; set; }

    /// <summary>
    /// 招贤失败次数
    /// </summary>
    public int RecruitmentFailureCount { get; set; }
    
    /// <summary>
    /// 被击破
    /// </summary>
    public bool Destroyed { get; set; }

    public int SecondTierXResidue { get; set; }

    public int SecondTierYResidue { get; set; }

    public int ThirdTierXResidue { get; set; }

    public int ThirdTierYResidue { get; set; }
}