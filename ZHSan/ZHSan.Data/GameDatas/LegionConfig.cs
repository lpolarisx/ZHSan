
using System.Collections.Generic;
using GameEnums;
using Microsoft.Xna.Framework;

namespace GameDatas;

public class LegionConfig
{
    public int Id { get; set; }

    public LegionKind Kind { get; set; }

    public int CoreTroopString { get; set; }

    public Point? InformationDestination { get; set; }

    public int PreferredRoutewayString { get; set; }

    public int StartArchitectureString { get; set; }

    public List<Point> TakenPositions { get; set; } = new();

    public string TroopsString { get; set; }

    public int WillArchitectureString { get; set; }
}