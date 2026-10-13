
namespace GameDatas;

public class TitleKindConfig : BaseConfig
{
    /// <summary>
    /// 战斗
    /// </summary>
    public bool Combat { get; set; }

    /// <summary>
    /// 习得天数
    /// </summary>
    public int StudyDay { get; set; }

    /// <summary>
    /// 习得成功率
    /// </summary>
    public int SuccessRate { get; set; }

    /// <summary>
    /// 可免除
    /// </summary>
    public bool Recallable { get; set; }

    /// <summary>
    /// 可额外传授
    /// </summary>
    public bool RandomTeachable { get; set; }
}