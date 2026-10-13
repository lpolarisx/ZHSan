using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6420 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        var disasterId = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        if (arch.disasterChanceDecrease.ContainsKey(disasterId))
        {
            arch.disasterChanceDecrease[disasterId] += increment;
        }
        else
        {
            arch.disasterChanceDecrease[disasterId] = increment;
        }
    }


    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        var disasterId = influence.GetIntParam();

        if (arch.disasterChanceDecrease.ContainsKey(disasterId))
        {
            arch.disasterChanceDecrease[disasterId] -= influence.GetIntParam2();
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return influence.GetIntParam2() / 10;
    }
}