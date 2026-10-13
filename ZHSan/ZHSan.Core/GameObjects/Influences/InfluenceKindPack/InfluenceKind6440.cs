using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6440 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.facilityConstructionTimeRateDecrease += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.facilityConstructionTimeRateDecrease -= influence.GetFloatParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var positionRate = arch.FacilityPositionLeft > 0 ? 1 : 0.01;
        var hostileLine = arch.HostileLine ? 2 : 1;

        return positionRate * hostileLine;
    }
}