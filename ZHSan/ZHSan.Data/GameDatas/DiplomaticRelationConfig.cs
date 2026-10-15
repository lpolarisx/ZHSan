
namespace GameDatas;

public class DiplomaticRelationConfig
{
    /// <summary>
    /// 势力-1
    /// </summary>
    public int RelationFaction1ID { get; set; }

    /// <summary>
    /// 势力-2
    /// </summary>
    public int RelationFaction2ID { get; set; }

    /// <summary>
    /// 外交关系
    /// </summary>
    public int Relation { get; set; }

    /// <summary>
    /// 停战天数
    /// </summary>
    public int Truce { get; set; }

    /// <summary>
    /// 本月谈判未成的次数，满了这个月就不再谈
    /// </summary>
    public int NegotiationCount { get; set; }

    /// <summary>
    /// AI再次主动遣使提议之前还要等的月数
    /// </summary>
    public int ProposalCooldown { get; set; }

    /// <summary>
    /// 解除同盟后还要等几个月才能再结盟
    /// </summary>
    public int AllianceCooldown { get; set; }
}