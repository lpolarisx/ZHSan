
using GameEnums;

namespace GameDatas;

public class GameDateConfig
{
    public int Year { get; set; }

    public int Month { get; set; } = 1;

    public int Day { get; set; } = 1;

    public GameSeason Season { get; set; }

    public int DaysLeft { get; set; }

    public bool IsRunning { get; set; }
}