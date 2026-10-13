
using GameObjects;

namespace GameEvents;

/// <summary>
/// 玩家释放俘虏
/// </summary>
/// <param name="from">玩家势力</param>
/// <param name="to">俘虏所属势力</param>
/// <param name="captive">俘虏</param>
public record PlayerReleaseEvent(Faction from, Faction to, Captive captive);

/// <summary>
/// 释放俘虏
/// </summary>
/// <param name="success">是否释放</param>
/// <param name="from">玩家势力</param>
/// <param name="to">俘虏所属势力</param>
/// <param name="person">人物</param>
public record ReleaseEvent(bool success, Faction from, Faction to, Person person);

/// <summary>
/// 主动释放俘虏
/// </summary>
/// <param name="captive">俘虏</param>
public record SelfReleaseEvent(Captive captive);

/// <summary>
/// 逃脱
/// </summary>
/// <param name="captive"></param>
public record EscapeEvent(Captive captive);