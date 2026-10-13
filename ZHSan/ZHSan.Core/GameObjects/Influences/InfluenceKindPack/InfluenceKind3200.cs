using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3200 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfAgriculturePerDay += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfAgriculturePerDay -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        if (!arch.Kind.HasAgriculture) return -1;

        return (arch.AgricultureCeiling - arch.Agriculture) * influence.GetIntParam() + 1;
    }
}