using GameObjects;
using GameObjects.FactionDetail;

namespace GameEvents;


/// <summary>
/// 领袖被捕获
/// </summary>
/// <param name="person">领袖</param>
/// <param name="faction">势力</param>
public record AfterCatchLeaderEvent(Person leader, Faction faction);

/// <summary>
/// 势力销毁
/// </summary>
/// <param name="faction">势力</param>
public record FactionDestoryEvent(Faction faction);

/// <summary>
/// 更新科技
/// </summary>
/// <param name="faction">势力</param>
/// <param name="technique">科技</param>
/// <param name="architecture">建筑</param>
public record UpgradeTechniqueEvent(Faction faction, Technique technique, Architecture architecture);

/// <summary>
/// 科技更新完成
/// </summary>
/// <param name="faction">势力</param>
/// <param name="technique">科技</param>
public record TechniqueFinishedEvent(Faction faction, Technique technique);

/// <summary>
/// 主动迁移都城
/// </summary>
/// <param name="faction">势力</param>
/// <param name="oldCapital">旧都城</param>
/// <param name="newCapital">新都城</param>
public record InitiativeChangeCapitalEvent(Faction faction, Architecture oldCapital, Architecture newCapital);

/// <summary>
/// 被动迁移都城
/// </summary>
/// <param name="faction">势力</param>
/// <param name="oldCapital">旧都城</param>
/// <param name="newCapital">新都城</param>
public record ForcedChangeCapitalEvent(Faction faction, Architecture oldCapital, Architecture newCapital);

/// <summary>
/// 获取控制权(轮到玩家)
/// </summary>
/// <param name="faction">势力</param>
public record GetControlEvent(Faction faction);