
using GameEnums;
using Microsoft.Xna.Framework;

namespace GameDatas;

public class InformationConfig
{
    public int Id { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    public InformationLevel Level { get; set; }

    /// <summary>
    /// 是否斜向
    /// </summary>
    public bool Oblique { get; set; }

    /// <summary>
    /// 消耗资金
    /// </summary>
    public int DayCost { get; set; }

    /// <summary>
    /// 剩余天数
    /// </summary>
    public int DaysLeft { get; set; }

    /// <summary>
    /// 已进行天数
    /// </summary>
    public int DaysStarted { get; set; }

    /// <summary>
    /// 位置
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// 范围
    /// </summary>
    public int Radius { get; set; }
}