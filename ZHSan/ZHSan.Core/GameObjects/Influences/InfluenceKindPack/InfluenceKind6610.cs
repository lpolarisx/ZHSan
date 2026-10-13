using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6610 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        var faction = arch.BelongedFaction;

        if (faction != null)
        {
            faction.techniquePointCostRateDecrease.Add(influence.GetFloatParam());
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        var faction = arch.BelongedFaction;

        if (faction != null)
        {
            faction.techniquePointCostRateDecrease.Remove(influence.GetFloatParam());
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return influence.GetFloatParam();
    }
}