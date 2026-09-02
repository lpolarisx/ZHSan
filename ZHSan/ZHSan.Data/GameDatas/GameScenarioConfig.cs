
using System.Collections.Generic;

namespace GameDatas;

public class GameScenarioConfig
{
    public string Mod { get; set; }

    public Dictionary<int, int[]> AIBattlingArchitectureStrings { get; set; }

    public Dictionary<int, int> FatherIds { get; set; }

    public Dictionary<int, int> MotherIds { get; set; }

    public Dictionary<int, int> SpouseIds { get; set; }

    public Dictionary<int, int[]> BrotherIds { get; set; }

    public Dictionary<int, int[]> SuoshuIds { get; set; }

    public Dictionary<int, int[]> CloseIds { get; set; }

    public Dictionary<int, int[]> HatedIds { get; set; }

    public Dictionary<int, int> MarriageGranterId { get; set; }

    public List<int> PlayerList { get; set; }

    public GameDateConfig Date { get; set; }

    public string CurrentPlayerID { get; set; }

    public string PlayerInfo { get; set; }

    public string ScenarioDescription { get; set; }

    public string ScenarioTitle { get; set; }

    public bool UsingOwnCommonData { get; set; }

    public int GameTime { get; set; }

    public int DaySince { get; set; }

    public MapConfig ScenarioMap { get; set; }

    public ParametersConfig Parameters { get; set; }

    public GlobalVariablesConfig GlobalVariables { get; set; }
}