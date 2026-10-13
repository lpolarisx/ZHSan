using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3530 : InfluenceKind
{
    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return arch.IsFundAbundant && arch.IsFoodAbundant ? arch.PersonCount : 0;
    }
}