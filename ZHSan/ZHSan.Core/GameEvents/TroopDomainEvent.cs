
using GameObjects;
using GameObjects.PersonDetail;
using GameObjects.TroopDetail;

namespace GameEvents;

/// <summary>
/// 部队创建
/// </summary>
/// <param name="troop">部队</param>
public record TroopCreateEvent(Troop troop);

/// <summary>
/// 开始寻路
/// </summary>
/// <param name="troop">部队</param>
public record StartPathEvent(Troop troop);

/// <summary>
/// 结束寻路
/// </summary>
/// <param name="troop">部队</param>
public record EndPathEvent(Troop troop);

/// <summary>
/// 路径未找到
/// </summary>
/// <param name="troop">部队</param>
public record PathNotFoundEvent(Troop troop);

/// <summary>
/// 普通攻击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record NormalAttackEvent(Troop attacker, Troop defender);

/// <summary>
/// 战法攻击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
/// <param name="combatMethod">战法</param>
public record CombatMethodAttackEvent(Troop attacker, Troop defender, CombatMethod combatMethod);

/// <summary>
/// 使用策略
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
/// <param name="stratagem">策略</param>
public record CastStratagemEvent(Troop attacker, Troop defender, Stratagem stratagem);

/// <summary>
/// 致命一击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record CriticalStrikeEvent(Troop attacker, Troop defender);

/// <summary>
/// 受到致命一击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record ReceiveCriticalStrikeEvent(Troop attacker, Troop defender);

/// <summary>
/// 伏击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record WaylayEvent(Troop attacker, Troop defender);

/// <summary>
/// 受到伏击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record ReceiveWaylayEvent(Troop attacker, Troop defender);

/// <summary>
/// 围攻
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record SurroundEvent(Troop attacker, Troop defender);

/// <summary>
/// 设置战法
/// </summary>
/// <param name="troop">部队</param>
/// <param name="combatMethod">战法</param>
public record SetCombatMethodEvent(Troop troop, CombatMethod combatMethod);

/// <summary>
/// 设置策略
/// </summary>
/// <param name="troop">部队</param>
/// <param name="stratagem">策略</param>
public record SetStratagemEvent(Troop troop, Stratagem stratagem);

/// <summary>
/// 策略成功
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
/// <param name="stratagem">策略</param>
/// <param name="isHarmful">是否造成伤害</param>
public record StratagemSuccessedEvent(Troop attacker, Troop defender, Stratagem stratagem, bool isHarmful);

/// <summary>
/// 混乱
/// </summary>
/// <param name="troop">部队</param>
/// <param name="deepChaos">深度混乱</param>
public record ChaosEvent(Troop troop, bool deepChaos);

/// <summary>
/// 伪报
/// </summary>
/// <param name="troop">部队</param>
public record RumourEvent(Troop troop);

/// <summary>
/// 挑衅
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record AttractEvent(Troop attacker, Troop defender);

/// <summary>
/// 从混乱中恢复
/// </summary>
/// <param name="troop"></param>
public record RecoverFromChaosEvent(Troop troop);

/// <summary>
/// 使用深度混乱
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record CastDeepChaosEvent(Troop attacker, Troop defender);

/// <summary>
/// 抵抗策略
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
/// <param name="stratagem">策略</param>
/// <param name="isHarmful">是否造成伤害</param>
public record ResistStratagemEvent(Troop attacker, Troop defender, Stratagem stratagem, bool isHarmful);

/// <summary>
/// 埋伏
/// </summary>
/// <param name="troop">部队</param>
public record AmbushEvent(Troop troop);

/// <summary>
/// 停止埋伏
/// </summary>
/// <param name="troop">部队</param>
public record StopAmbushEvent(Troop troop);

/// <summary>
/// 发现埋伏
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record DiscoverAmbushEvent(Troop attacker, Troop defender);

/// <summary>
/// 击破
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record RoutEvent(Troop attacker, Troop defender);

/// <summary>
/// 被击破
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record RoutedEvent(Troop attacker, Troop defender);

/// <summary>
/// 攻破城池
/// </summary>
/// <param name="troop">部队</param>
/// <param name="architecture">建筑</param>
public record BreakWallEvent(Troop troop, Architecture architecture);

/// <summary>
/// 火势蔓延
/// </summary>
/// <param name="troop">部队</param>
public record SpreadBurntEvent(Troop troop);

/// <summary>
/// 占领城池
/// </summary>
/// <param name="troop">部队</param>
/// <param name="architecture">建筑</param>
public record OccupyArchitectureEvent(Troop troop, Architecture architecture);

/// <summary>
/// 免疫攻击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record AntiAttackEvent(Troop attacker, Troop defender);

/// <summary>
/// 免疫箭矢攻击
/// </summary>
/// <param name="attacker">攻击方</param>
/// <param name="defender">防守方</param>
public record AntiArrowAttackEvent(Troop attacker, Troop defender);

/// <summary>
/// 征收粮草
/// </summary>
/// <param name="troop">部队</param>
/// <param name="grain">粮草</param>
public record LevyGrainEvent(Troop troop, int grain);

/// <summary>
/// 切断粮道
/// </summary>
/// <param name="troop">部队</param>
/// <param name="days">天数</param>
public record CutRoutewayEvent(Troop troop, int days);

/// <summary>
/// 切断粮道结果
/// </summary>
/// <param name="troop">部队</param>
/// <param name="success">是否成功</param>
public record CutRoutewayResultEvent(Troop troop, bool success);

/// <summary>
/// 获取新俘虏
/// </summary>
/// <param name="troop">部队</param>
/// <param name="persons">俘虏</param>
public record GetNewCaptiveEvent(Troop troop, PersonList persons);

/// <summary>
/// 释放部队俘虏
/// </summary>
/// <param name="troop">部队</param>
/// <param name="persons">俘虏</param>
public record ReleaseTroopCaptiveEvent(Troop troop, PersonList persons);

/// <summary>
/// 单挑
/// </summary>
/// <param name="win"></param>
/// <param name="attackingTroop">进攻部队</param>
/// <param name="attacker">攻击方</param>
/// <param name="defenseTroop">防守部队</param>
/// <param name="defender">防守方</param>
public record PersonChallengeEvent(int win, Troop attackingTroop, Person attacker, Troop defenseTroop, Person defender);

/// <summary>
/// 论战
/// </summary>
/// <param name="win">是否获胜</param>
/// <param name="attackingTroop">进攻部队</param>
/// <param name="attacker">攻击方</param>
/// <param name="defenseTroop">防守部队</param>
/// <param name="defender">防守方</param>
public record PersonControversyEvent(bool win, Troop attackingTroop, Person attacker, Troop defenseTroop, Person defender);

/// <summary>
/// 爆发
/// </summary>
/// <param name="troop">部队</param>
/// <param name="kind">爆发种类</param>
public record OutburstEvent(Troop troop, OutburstKind kind);

/// <summary>
/// 应用特技
/// </summary>
/// <param name="troop">部队</param>
/// <param name="stunt">特技</param>
public record ApplyStuntEvent(Troop troop, Stunt stunt);

/// <summary>
/// 运输到达
/// </summary>
/// <param name="troop">部队</param>
/// <param name="architecture">建筑</param>
public record TransportArrivedEvent(Troop troop, Architecture architecture);

/// <summary>
/// 进入势力范围
/// </summary>
/// <param name="troop">部队</param>
/// <param name="faction">势力</param>
public record EnterFactionAreaEvent(Troop troop, Faction faction);

/// <summary>
/// 离开势力范围
/// </summary>
/// <param name="troop">部队</param>
/// <param name="faction">势力</param>
public record LeaveFactionAreaEvent(Troop troop, Faction faction);

/// <summary>
/// 发现部队
/// </summary>
/// <param name="troop">部队</param>
/// <param name="discoveredTroop">被发现部队</param>
public record TroopFound(Troop troop, Troop discoveredTroop);

/// <summary>
/// 部队消失
/// </summary>
/// <param name="troop">部队</param>
/// <param name="disappearedTroop">消失的部队</param>
public record TroopLost(Troop troop, Troop disappearedTroop);

/// <summary>
/// 应用部队事件
/// </summary>
/// <param name="troopEvent">部队事件</param>
/// <param name="troop">部队</param>
public record ApplyTroopEvent(TroopEvent troopEvent, Troop troop);