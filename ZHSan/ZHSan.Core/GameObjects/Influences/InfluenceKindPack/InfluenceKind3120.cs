using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3120 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.CombativityOfRecruitment += influence.GetIntParam() - 50;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.CombativityOfRecruitment -= influence.GetIntParam() - 50;
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var frontLineFirst = arch.FrontLine ? (influence.GetIntParam() - 50) / 25.0 : 0.01;
        var frontLineSecond = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1; 

        return frontLineFirst * frontLineSecond * hostileLine * criticalHostile;
    }
}