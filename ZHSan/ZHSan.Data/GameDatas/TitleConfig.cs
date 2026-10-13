
namespace GameDatas;

public class TitleConfig : BaseConfig
{
    /// <summary>
    /// 类别Id
    /// </summary>
    public int KindId { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 战斗
    /// </summary>
    public bool Combat { get; set; }

    /// <summary>
    /// 手动授予
    /// </summary>
    public bool ManualAward { get; set; }

    /// <summary>
    /// 薪金
    /// </summary>
    public int FundForHolder { get; set; }

    /// <summary>
    /// 影响列表
    /// </summary>
    public string InfluencesString { get; set; }

    /// <summary>
    /// 条件列表
    /// </summary>
    public string ConditionTableString { get; set; }

    /// <summary>
    /// 生成武將条件
    /// </summary>
    public string GenerateConditionsString { get; set; }

    /// <summary>
    /// 建筑条件
    /// </summary>
    public string ArchitectureConditionsString { get; set; }

    /// <summary>
    /// 势力条件
    /// </summary>
    public string FactionConditionsString { get; set; }

    /// <summary>
    /// 失去条件
    /// </summary>
    public string LoseConditionsString { get; set; }

    /// <summary>
    /// 自动习得机率：每天有1除以此数的机率自动习得这个称号。0为不会自动习得
    /// </summary>
    public int AutoLearn { get; set; }

    /// <summary>
    /// 习得对话
    /// </summary>
    public string AutoLearnText { get; set; }

    /// <summary>
    /// 习得传令官对话
    /// </summary>
    public string AutoLearnTextByCourier { get; set; }

    /// <summary>
    /// 全地图数目上限
    /// </summary>
    public int MapLimit { get; set; }

    /// <summary>
    /// 势力数目上限
    /// </summary>
    public int FactionLimit { get; set; }

    /// <summary>
    /// 继承机率
    /// </summary>
    public int InheritChance { get; set; }

    /// <summary>
    /// 不同生成武将类型获得机率
    /// </summary>
    public int[] GenerationChance { get; set; } = new int[10];
}