using GameDatas;
using GameEnums;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects;

[DataContract]
public class RoutePoint
{
    [DataMember]
    public int ActiveFundCost;

    public Routeway BelongedRouteway;

    [DataMember]
    public int BuildFundCost;

    [DataMember]
    public int BuildWorkCost;

    [DataMember]
    public float ConsumptionRate;

    [DataMember]
    public SimpleDirection Direction;

    [DataMember]
    public int Index;

    [DataMember]
    public Point Position;

    [DataMember]
    public SimpleDirection PreviousDirection;

    public RoutePoint() {}

    public RoutePoint(RoutePointConfig config)
    {
        ActiveFundCost = config.ActiveFundCost;
        BuildFundCost = config.BuildFundCost;
        BuildWorkCost = config.BuildWorkCost;
        ConsumptionRate = config.ConsumptionRate;
        Direction = config.Direction;
        Index = config.Index;
        Position = config.Position;
        PreviousDirection = config.PreviousDirection;
    }

    public RoutePointConfig ToConfig()
    {
        return new RoutePointConfig
        {
            ActiveFundCost = ActiveFundCost,
            BuildFundCost = BuildFundCost,
            BuildWorkCost = BuildWorkCost,
            ConsumptionRate = ConsumptionRate,
            Direction = Direction,
            Index = Index,
            Position = Position,
            PreviousDirection = PreviousDirection,
        };
    }
}