
using GameObjects;
using GameObjects.PersonDetail;

namespace GameEvents;

/// <summary>
/// 劫狱成功
/// </summary>
/// <param name="person">劫狱人</param>
/// <param name="captive">俘虏</param>
public record JailBreakSuccessedEvent(Person person, Captive captive);

/// <summary>
/// 劫狱失败
/// </summary>
/// <param name="person">劫狱人</param>
/// <param name="architecture">建筑</param>
public record JailBreakFailedEvent(Person person, Architecture architecture);

/// <summary>
/// 说服成功
/// </summary>
/// <param name="source">说服人</param>
/// <param name="target">被说服人</param>
/// <param name="faction">势力</param>
public record ConvinceSuccessedEvent(Person source, Person target, Faction faction);

/// <summary>
/// 说服失败
/// </summary>
/// <param name="source">说服人</param>
/// <param name="target">被说服人</param>
public record ConvinceFailedEvent(Person source, Person target);

/// <summary>
/// 情报获取
/// </summary>
/// <param name="person">人物</param>
/// <param name="information">情报</param>
public record InformationAcquisitionSuccessedEvent(Person person, Information information);

/// <summary>
/// 情报获取失败
/// </summary>
/// <param name="person">人物</param>
public record InformationAcquisitionFailedEvent(Person person);

/// <summary>
/// 间谍行动成功
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record SpyingSuccessedEvent(Person person, Architecture architecture);

/// <summary>
/// 间谍行动失败
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record SypingFailedEvent(Person person, Architecture architecture);

/// <summary>
/// 破坏成功
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
/// <param name="down"></param>
public record DestroySuccessedEvent(Person person, Architecture architecture, int down);

/// <summary>
/// 破坏失败
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record DestroyFailedEvent(Person person, Architecture architecture);

/// <summary>
/// 煽动成功
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
/// <param name="down"></param>
public record InstigateSuccessedEvent(Person person, Architecture architecture, int down);

/// <summary>
/// 煽动失败
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record InstigateFailedEvent(Person person, Architecture architecture);

/// <summary>
/// 流言成功
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record GossipSuccessedEvent(Person person, Architecture architecture);

/// <summary>
/// 流言失败
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record GossipFailedEvent(Person person, Architecture architecture);

/// <summary>
/// 搜索完成
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
/// <param name="result">结果</param>
public record SearchFinishedEvent(Person person, Architecture architecture, SearchResultPack result);

/// <summary>
/// 发现间谍
/// </summary>
/// <param name="person">人物</param>
/// <param name="spier">间谍</param>
public record SpierFoundEvent(Person person, Person spier);

/// <summary>
/// 发现宝物
/// </summary>
/// <param name="person">人物</param>
/// <param name="treasure">宝物</param>
public record TreasureFoundEvent(Person person, Treasure treasure);

/// <summary>
/// 显示消息
/// </summary>
/// <param name="person">人物</param>
/// <param name="message">消息</param>
public record ShowMessageEvent(Person person, PersonMessage message);

/// <summary>
/// 死亡
/// </summary>
/// <param name="person">人物</param>
/// <param name="killer">杀手</param>
/// <param name="architecture">建筑</param>
/// <param name="troop">部队</param>
public record DeathEvent(Person person, Person killer, Architecture architecture, Troop troop);

/// <summary>
/// 离开
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record LeaveEvent(Person person, Architecture architecture);

/// <summary>
/// 被谋杀
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record BeKilledEvent(Person person, Architecture architecture);

/// <summary>
/// 变更势力领袖
/// </summary>
/// <param name="faction">势力</param>
/// <param name="leader">领袖</param>
/// <param name="changeName">是否变更名称</param>
/// <param name="oldName">旧名称</param>
public record DeathChangeLeaderEvent(Faction faction, Person leader, bool changeName, string oldName);

/// <summary>
/// 变更势力
/// </summary>
/// <param name="dead">死者</param>
/// <param name="leader">领袖</param>
/// <param name="oldName">旧名称</param>
public record DeathChangeFactionEvent(Person dead, Person leader, string oldName);

/// <summary>
/// 学习称号完成
/// </summary>
/// <param name="person">人物</param>
/// <param name="title">称号</param>
/// <param name="success">是否成功</param>
public record StudyTitleFinishedEvent(Person person, Title title, bool success);

/// <summary>
/// 学习技能完成
/// </summary>
/// <param name="person">人物</param>
/// <param name="skillString">技能字符串</param>
/// <param name="success">是否成功</param>
public record StudySkillFinishedEvent(Person person, string skillString, bool success);

/// <summary>
/// 学习特技完成
/// </summary>
/// <param name="person">人物</param>
/// <param name="stunt">特技</param>
/// <param name="success">是否成功</param>
public record StudyStuntFinishedEvent(Person person, Stunt stunt, bool success);

/// <summary>
/// 赏赐宝物
/// </summary>
/// <param name="person">人物</param>
/// <param name="treasure">宝物</param>
public record AwardedTreasureEvent(Person person, Treasure treasure);

/// <summary>
/// 没收宝物
/// </summary>
/// <param name="person">人物</param>
/// <param name="treasure">宝物</param>
public record ConfiscatedTreasureEvent(Person person, Treasure treasure);

/// <summary>
/// 被建筑俘虏
/// </summary>
/// <param name="person">人物</param>
/// <param name="architecture">建筑</param>
public record CapturedByArchitectureEvent(Person person, Architecture architecture);

/// <summary>
/// 建立兄弟关系
/// </summary>
/// <param name="p1">人物1</param>
/// <param name="p2">人物2</param>
public record CreateBrotherEvent(Person p1, Person p2);

/// <summary>
/// 建立姐妹关系
/// </summary>
/// <param name="p1">人物1</param>
/// <param name="p2">人物2</param>
public record CreateSisterEvent(Person p1, Person p2);

/// <summary>
/// 建立配偶关系
/// </summary>
/// <param name="p1"></param>
/// <param name="p2"></param>
public record CreateSpouseEvent(Person p1, Person p2);
