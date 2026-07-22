using System.Runtime.Serialization;
using GameDatas;

namespace GameObjects.PersonDetail;

/// <summary>
/// 性格种类
/// </summary>
[DataContract]
public class CharacterKind : GameObject
{
    /// <summary>
    /// 单挑机率
    /// </summary>
    [DataMember]
    public int ChallengeChance { get; set; }

    /// <summary>
    /// 论战机率
    /// </summary>
    [DataMember]
    public int ControversyChance { get; set; }

    /// <summary>
    /// 智力比率
    /// </summary>
    [DataMember]
    public float IntelligenceRate { get; set; }

    /// <summary>
    /// 生成机率
    /// </summary>
    [DataMember]
    public int[] GenerationChance { get; set; } = new int[10];

    public CharacterKind(CharacterKindConfig config)
    {
        ID = config.Id;
        Name = config.Name;
        ChallengeChance = config.ChallengeChance;
        ControversyChance = config.ControversyChance;
        IntelligenceRate = config.IntelligenceRate;
        GenerationChance = config.GenerationChance;
    }
}