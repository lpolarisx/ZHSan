using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3270 : InfluenceKind
{
    public override void DoWork(Influence influence, Architecture arch)
    {
        var kindId = influence.GetIntParam();
        var rate = influence.GetFloatParam2();

        foreach (Military military in arch.Militaries)
        {
            if (military.Kind.ID == kindId && military.InjuryQuantity == 0)
            {
                arch.RecruitmentMilitary(military, rate);
            }
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var frontLineFirst = arch.FrontLine ? 1 : 0.001;
        var frontLineSecond = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return frontLineFirst * frontLineSecond * hostileLine * criticalHostile * influence.GetFloatParam2() * 10;
    }
}