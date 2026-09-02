using GameObjects;

namespace GameEvents;

/// <summary>
/// 加载剧本后
/// </summary>
public record AfterLoadScenarioEvent();

/// <summary>
/// 保存剧本后
/// </summary>
public record AfterSaveScenarioEvent();

/// <summary>
/// 出现新势力
/// </summary>
/// <param name="faction">势力</param>
public record NewFactionAppearEvent(Faction faction);