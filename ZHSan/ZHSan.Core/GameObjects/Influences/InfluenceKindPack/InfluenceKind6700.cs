using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6700 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.CommandDecrease.Add(new KeyValuePair<int, int>(prob, increment));
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.CommandDecrease.Remove(new KeyValuePair<int, int>(prob, increment));
    }
}