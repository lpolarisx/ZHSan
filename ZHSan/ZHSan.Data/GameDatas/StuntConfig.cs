
namespace GameDatas;

public class StuntConfig : BaseConfig
{
    /// <summary>
    /// 消耗战意
    /// </summary>
    public int Combativity { get; set; }

    /// <summary>
    /// 延续天数
    /// </summary>
    public int Period { get; set; }

    /// <summary>
    /// 动画
    /// </summary>
    public int Animation { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 使用条件列表
    /// </summary>
    public string CastConditionsString { get; set; }

    /// <summary>
    /// 修习条件列表
    /// </summary>
    public string LearnConditionsString { get; set; }

    /// <summary>
    /// AI触发条件
    /// </summary>
    public string AIConditionsString { get; set; }

    /// <summary>
    /// 生成武将条件
    /// </summary>
    public string GenerateConditionsString { get; set; }

    /// <summary>
    /// 不同生成武将类型获得机率
    /// </summary>
    public int[] GenerationChance { get; set; } = new int[10];
    
    /// <summary>
    /// 此技能的相关能力、0-4为武统智政魅
    /// </summary>
    public int RelatedAbility { get; set; }
}