using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind3080 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.NoCounterStrikeInArchitecture = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        arch.NoCounterStrikeInArchitecture = false;
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        if (!arch.FrontLine) return -1;

        int count = 0;
        foreach (Military m in arch.Militaries)
        {
            if (m.Kind.BeCountered)
            {
                count++;
            }
        }

        var frontLine = arch.FrontLine ? 2 : 1;
        var hostileLine = arch.HostileLine ? 2 : 1;
        var criticalHostile = arch.CriticalHostile ? 2 : 1;

        return (double)count / arch.EffectiveMilitaryCount * 2 * frontLine * hostileLine * criticalHostile;
    }
}