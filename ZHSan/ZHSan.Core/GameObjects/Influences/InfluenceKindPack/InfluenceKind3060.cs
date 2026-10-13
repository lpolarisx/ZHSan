using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3060 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfCombativityInViewArea += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.IncrementOfCombativityInViewArea -= influence.GetIntParam();
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var increment = influence.GetIntParam();
        var frontLineFirst = arch.FrontLine ? 1 : 0.001;
        var frontLineSecond = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return increment / 2.0 * frontLineFirst * frontLineSecond * hostileLine * criticalHostile;
    }
}