using System.Collections.Generic;

namespace GameDatas;

/// <summary>
/// 谈成的外交条款，使者带着它出发，到达对方都城时交割
/// </summary>
public class DiplomacyDealConfig
{
    /// <summary>
    /// 提议方
    /// </summary>
    public int SourceFactionID { get; set; }

    /// <summary>
    /// 接受方
    /// </summary>
    public int TargetFactionID { get; set; }

    /// <summary>
    /// 提议方出钱粮的城池
    /// </summary>
    public int SourceArchitectureID { get; set; } = -1;

    /// <summary>
    /// 随行的使者，交割时一同领赏
    /// </summary>
    public List<int> CompanionIDs { get; set; } = new();

    public int GiveFund { get; set; }

    public int GiveFood { get; set; }

    /// <summary>
    /// 提议方割让的城池
    /// </summary>
    public List<int> GiveArchitectureIDs { get; set; } = new();

    public int RequestFund { get; set; }

    public int RequestFood { get; set; }

    /// <summary>
    /// 提议方索要的城池
    /// </summary>
    public List<int> RequestArchitectureIDs { get; set; } = new();

    /// <summary>
    /// 双方停战天数，0表示不停战
    /// </summary>
    public int TruceDays { get; set; }

    /// <summary>
    /// 双方结盟
    /// </summary>
    public bool Alliance { get; set; }
}
