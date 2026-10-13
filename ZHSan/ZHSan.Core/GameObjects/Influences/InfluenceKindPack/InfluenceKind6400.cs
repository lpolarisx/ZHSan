using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6400 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.noFundToSustainFacility = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.noFundToSustainFacility = false;
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return arch.FacilityMaintenanceCost / 5 - 1;
    }

}