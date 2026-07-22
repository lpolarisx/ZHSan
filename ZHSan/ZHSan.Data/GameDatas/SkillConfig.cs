
namespace GameDatas;

public class SkillConfig : BaseConfig
{
    /// <summary>
    /// 显示行
    /// </summary>
    public int DisplayRow { get; set; }

    /// <summary>
    /// 显示列
    /// </summary>
    public int DisplayCol { get; set; }

    /// <summary>
    /// 类别
    /// </summary>
    public int Kind { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 战斗
    /// </summary>
    public bool Combat { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 条件列表
    /// </summary>
    public string ConditionTableString { get; set; }

    /// <summary>
    /// 不同生成武将类型获得机率
    /// </summary>
    public int[] GenerationChance { get; set; } = new int[10];

    /// <summary>
    /// 此技能的相关能力、0-4为武统智政魅
    /// </summary>
    public int RelatedAbility { get; set; }
}