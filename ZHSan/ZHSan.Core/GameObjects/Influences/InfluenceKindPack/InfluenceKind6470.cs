using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6470 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.enduranceDecreaseRateDrop += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.enduranceDecreaseRateDrop -= influence.GetFloatParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var hostileLine = arch.HostileLine ? 2 : 1;

        return influence.GetFloatParam() * hostileLine;
    }
}