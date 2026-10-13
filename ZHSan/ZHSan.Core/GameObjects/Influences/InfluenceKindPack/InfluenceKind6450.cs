using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6450 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        var disasterId = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        foreach (LinkNode i in arch.AIAllLinkNodes.Values)
        {
            var dict = i.A.disasterChanceIncrease;

            if (dict.ContainsKey(disasterId))
            {
                dict[disasterId] += increment;
            }
            else
            {
                dict[disasterId] = increment;
            }
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        var disasterId = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        foreach (LinkNode i in arch.AIAllLinkNodes.Values)
        {
            i.A.disasterChanceIncrease[disasterId] -= increment;
        }
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        return influence.GetIntParam2() * 1000;
    }
}