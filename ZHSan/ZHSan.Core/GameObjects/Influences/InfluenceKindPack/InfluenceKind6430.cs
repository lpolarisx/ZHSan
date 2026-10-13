using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6430 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        var disasterId = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        if (arch.disasterDamageRateDecrease.ContainsKey(disasterId))
        {
            arch.disasterDamageRateDecrease[disasterId] += increment;
        }
        else
        {
            arch.disasterDamageRateDecrease[disasterId] = increment;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        var disasterId = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        if (arch.disasterDamageRateDecrease.ContainsKey(disasterId))
        {
            arch.disasterDamageRateDecrease[disasterId] -= increment;
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return influence.GetIntParam2() / 10;
    }
}