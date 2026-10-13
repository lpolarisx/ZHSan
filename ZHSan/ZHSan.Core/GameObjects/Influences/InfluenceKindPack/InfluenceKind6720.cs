using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6720 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.IntelligenceDecrease.Add(new KeyValuePair<int, int>(prob, increment));
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.IntelligenceDecrease.Remove(new KeyValuePair<int, int>(prob, increment));
    }
}