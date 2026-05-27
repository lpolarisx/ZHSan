using System;
using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind352 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Troop troop)
    {
        var rate = influence.GetFloatParam();
        var limit = influence.GetFloatParam2() - troop.WaterRate;

        if (limit >= 0)
        {
            troop.RateIncrementOfTerrainRateOnWater = Math.Min(limit, rate);
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Troop troop)
    {
        troop.RateIncrementOfTerrainRateOnWater = 0;
    }
}