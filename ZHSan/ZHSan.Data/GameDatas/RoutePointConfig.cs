
using GameEnums;
using Microsoft.Xna.Framework;

namespace GameDatas;

public class RoutePointConfig
{
    public int ActiveFundCost { get; set; }

    public int BuildFundCost { get; set; }

    public int BuildWorkCost { get; set; }

    public float ConsumptionRate { get; set; }

    public SimpleDirection Direction { get; set; }

    public int Index { get; set; }

    public Point Position { get; set; }

    public SimpleDirection PreviousDirection { get; set; }
}