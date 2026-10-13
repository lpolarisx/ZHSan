using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3230 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.IncrementOfDominationPerDay += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture architecture)
    {
        architecture.IncrementOfDominationPerDay -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture a)
    {
        if (!a.Kind.HasDomination) return -1;
        return (a.DominationCeiling - a.Domination) * influence.GetIntParam() + 1;
    }
}