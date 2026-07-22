
using System.ComponentModel;

namespace GameEnums;

public enum InformationLevel
{
    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    Unknown,

    /// <summary>
    /// 无
    /// </summary>
    [Description("无")]
    None,

    /// <summary>
    /// 低
    /// </summary>
    [Description("低")]
    Low,

    /// <summary>
    /// 中
    /// </summary>
    [Description("中")]
    Medium,

    /// <summary>
    /// 高
    /// </summary>
    [Description("高")]
    High,

    /// <summary>
    /// 全
    /// </summary>
    [Description("全")]
    Full
}