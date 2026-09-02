using System.Collections.Generic;
using GameObjects;

namespace GameEvents;

/// <summary>
/// 宴请事件
/// </summary>
/// <param name="inviter">邀请人</param>
/// <param name="invitees">受邀人</param>
public record EntertainEvent(Person inviter, List<Person> invitees);

/// <summary>
/// 任命县令
/// </summary>
/// <param name="leader">领袖</param>
/// <param name="person">人物</param>
public record AppointMayorEvent(Person leader, Person person);

/// <summary>
/// 招贤
/// </summary>
/// <param name="leader">领袖</param>
/// <param name="person">人物</param>
public record ZhaoXianEvent(Person leader, Person person);

/// <summary>
/// 立储
/// </summary>
/// <param name="leader">领袖</param>
/// <param name="person">人物</param>
public record SelectPrinceEvent(Person leader, Person person);

/// <summary>
/// 人口流失
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="quantity">数量</param>
public record PopulationEscapeEvent(Architecture architecture, int quantity);

/// <summary>
/// 人口流入
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="quantity">数量</param>
public record PopulationEnterEvent(Architecture architecture, int quantity);

/// <summary>
/// 最近被攻击
/// </summary>
/// <param name="architecture">建筑</param>
public record RecentlyAttackedEvent(Architecture architecture);

/// <summary>
/// 释放俘虏
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="persons">俘虏</param>
public record ReleaseCaptiveEvent(Architecture architecture, PersonList persons);

/// <summary>
/// 发生灾难
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="disasterId">灾难Id</param>
public record DisasterOccurredEvent(Architecture architecture, int disasterId);

/// <summary>
/// 设施竣工
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="facility">设施</param>
public record FacilityCompletedEvent(Architecture architecture, Facility facility);

/// <summary>
/// 褒奖
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="persons">人物</param>
public record RewardPersonEvent(Architecture architecture, PersonList persons);

/// <summary>
/// 聘用
/// </summary>
/// <param name="persons">人物</param>
public record HirePersonEvent(PersonList persons);

/// <summary>
/// 军队创建
/// </summary>
/// <param name="architecture">建筑</param>
/// <param name="military">军队</param>
public record MilitaryCreateEvent(Architecture architecture, Military military);