
using GameEnums;

namespace GameEvents;

/// <summary>
/// 季节变化
/// </summary>
/// <param name="season">季节</param>
public record SeasonChangeEvent(GameSeason season);
