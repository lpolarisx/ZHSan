using Microsoft.Xna.Framework;

namespace GameEvents;

/// <summary>
/// 按下鼠标左键
/// </summary>
/// <param name="point">位置</param>
public record MouseLeftDownEvent(Point point);

/// <summary>
/// 释放鼠标左键
/// </summary>
/// <param name="point">位置</param>
public record MouseLeftUpEvent(Point point);

/// <summary>
/// 按下鼠标右键
/// </summary>
/// <param name="point">位置</param>
public record MouseRightDownEvent(Point point);

/// <summary>
/// 释放鼠标右键
/// </summary>
/// <param name="point">位置</param>
public record MouseRightUpEvent(Point point);

/// <summary>
/// 移动鼠标
/// </summary>
/// <param name="point">位置</param>
/// <param name="leftDown">是否按下左键</param>
public record MouseMoveEvent(Point point, bool leftDown);

/// <summary>
/// 滚动鼠标
/// </summary>
/// <param name="point">位置</param>
/// <param name="scrollValue">滚动值</param>
public record MouseScrollEvent(Point point, int scrollValue);