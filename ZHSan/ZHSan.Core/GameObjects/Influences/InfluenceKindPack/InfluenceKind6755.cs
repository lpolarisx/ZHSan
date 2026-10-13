using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6755 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.ReputationIncrease.Add(new KeyValuePair<int, int>(prob, increment));
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.ReputationIncrease.Remove(new KeyValuePair<int, int>(prob, increment));
    }
}