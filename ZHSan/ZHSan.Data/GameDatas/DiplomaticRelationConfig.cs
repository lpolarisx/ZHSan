
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
}