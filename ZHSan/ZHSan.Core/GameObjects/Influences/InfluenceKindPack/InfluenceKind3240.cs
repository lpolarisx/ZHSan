using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3240 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfMoralePerDay += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfMoralePerDay -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        if (!arch.Kind.HasMorale) return -1;

        return (arch.MoraleCeiling - arch.Morale) * influence.GetIntParam() + 1;
    }
}