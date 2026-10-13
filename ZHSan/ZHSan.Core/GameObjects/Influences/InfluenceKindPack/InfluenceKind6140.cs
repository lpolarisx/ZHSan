using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6140 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Architecture arch)
    {
        var threshold = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        arch.captiveLoyaltyFall.Add(new KeyValuePair<int, int>(threshold, increment));
    }

    public override void PurifyInfluenceKind(Influence influence, Architecture arch)
    {
        var threshold = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        arch.captiveLoyaltyFall.Remove(new KeyValuePair<int, int>(threshold, increment));
    }

    public override double AIFacilityValue(Influence influence, Architecture arch)
    {
        var threshold = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        if (threshold > 110 && arch.FrontLine) return 100;

        var frontLine = arch.FrontLine ? 1 : 0.2;

        return increment * threshold * 2 / (double)arch.BelongedFaction.PersonCount * frontLine;
    }
}