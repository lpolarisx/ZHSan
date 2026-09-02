using GameObjects;

namespace GameEvents;

/// <summary>
/// 应用游戏内事件
/// </summary>
/// <param name="ingameEvent">游戏事件</param>
/// <param name="architecture">建筑</param>
/// <param name="screen">屏幕</param>
public record ApplyIngameEvent(Event ingameEvent, Architecture architecture, Screen screen);