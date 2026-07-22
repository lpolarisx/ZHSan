
namespace GameDatas;

public class CharacterKindConfig : BaseConfig
{
    /// <summary>
    /// 单挑机率
    /// </summary>
    public int ChallengeChance { get; set; }

    /// <summary>
    /// 论战机率
    /// </summary>
    public int ControversyChance { get; set; }

    /// <summary>
    /// 智力比率
    /// </summary>
    public float IntelligenceRate { get; set; }

    /// <summary>
    /// 生成机率
    /// </summary>
    public int[] GenerationChance { get; set; } = new int[10];
}