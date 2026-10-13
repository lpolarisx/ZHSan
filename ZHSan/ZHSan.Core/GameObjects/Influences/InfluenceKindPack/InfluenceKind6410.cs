using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6410 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.facilityEnduranceIncrease += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.facilityEnduranceIncrease -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        if (arch.FacilityCount <= 0) return -1;

        var frontLineFirst = arch.FrontLine ? (1 - influence.GetIntParam()) * 2 : 0.01;
        var frontLineSecond = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return frontLineFirst * frontLineSecond * hostileLine * criticalHostile;
    }
}