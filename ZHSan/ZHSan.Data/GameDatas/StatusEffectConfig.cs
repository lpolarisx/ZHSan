
namespace GameDatas;

public class StatusEffectConfig : BaseConfig
{
    /// <summary>
    /// 延续天数
    /// </summary>
    public int Duration { get; set; }

    /// <summary>
    /// 状态类型
    /// </summary>
    public int StatusType { get; set; }

    /// <summary>
    /// 触发条件
    /// </summary>
    public string TriggerConditions { get; set; }

    /// <summary>
    /// 影响
    /// </summary>
    public string Influences { get; set; }
}