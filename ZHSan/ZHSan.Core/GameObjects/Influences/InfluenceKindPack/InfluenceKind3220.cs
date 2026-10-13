using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3220 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfTechnologyPerDay += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfTechnologyPerDay -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        if (!arch.Kind.HasTechnology) return -1;

        return (arch.TechnologyCeiling - arch.Technology) * influence.GetIntParam() + 1;
    }
}