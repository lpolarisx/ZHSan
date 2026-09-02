using GameManager;
using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3050 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        var faction = arch.BelongedFaction;

        if (faction != null)
        {
            faction.RemoveArchitectureKnownData(arch);

            if (arch.AutoRefillFoodInLongViewArea)
            {
                foreach (Point point in arch.LongViewArea.Area)
                {
                    if (!Session.Current.Scenario.PositionOutOfRange(point))
                    {
                        Session.Current.Scenario.MapTileData[point.X, point.Y].RemoveSupplyingArchitecture(arch);
                    }
                }
            }
            arch.IncrementOfViewRadius += influence.GetIntParam();
            arch.ViewArea = null;
            arch.LongViewArea = null;
            if (!Session.Current.Scenario.Preparing)
            {
                foreach (var architecture2 in Session.Current.Scenario.Architectures.Values)
                {
                    architecture2.RefreshViewArea();
                }
                foreach (Troop troop in Session.Current.Scenario.Troops)
                {
                    troop.RefreshViewArchitectureRelatedArea();
                }
            }

            if (arch.AutoRefillFoodInLongViewArea)
            {
                foreach (Point point in arch.LongViewArea.Area)
                {
                    if (!Session.Current.Scenario.PositionOutOfRange(point))
                    {
                        Session.Current.Scenario.MapTileData[point.X, point.Y].AddSupplyingArchitecture(arch);
                    }
                }
            }
            faction.AddArchitectureKnownData(arch);
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        var faction = arch.BelongedFaction;

        if (faction != null)
        {
            faction.RemoveArchitectureKnownData(arch);
            if (arch.AutoRefillFoodInLongViewArea)
            {
                foreach (Point point in arch.LongViewArea.Area)
                {
                    if (!Session.Current.Scenario.PositionOutOfRange(point))
                    {
                        Session.Current.Scenario.MapTileData[point.X, point.Y].RemoveSupplyingArchitecture(arch);
                    }
                }
            }
            arch.IncrementOfViewRadius -= influence.GetIntParam();
            arch.ViewArea = null;
            arch.LongViewArea = null;
            if (!Session.Current.Scenario.Preparing)
            {
                foreach (var architecture2 in Session.Current.Scenario.Architectures.Values)
                {
                    architecture2.RefreshViewArea();
                }
                foreach (Troop troop in Session.Current.Scenario.Troops)
                {
                    troop.RefreshViewArchitectureRelatedArea();
                }
            }
            if (arch.AutoRefillFoodInLongViewArea)
            {
                foreach (Point point in arch.LongViewArea.Area)
                {
                    if (!Session.Current.Scenario.PositionOutOfRange(point))
                    {
                        Session.Current.Scenario.MapTileData[point.X, point.Y].AddSupplyingArchitecture(arch);
                    }
                }
            }
            faction.AddArchitectureKnownData(arch);
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var increment = influence.GetIntParam();
        var frontLineFirst = arch.FrontLine ? 1 : 0.001;
        var frontLineSecond = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return increment * increment * frontLineFirst * frontLineSecond * hostileLine * criticalHostile;
    }
}