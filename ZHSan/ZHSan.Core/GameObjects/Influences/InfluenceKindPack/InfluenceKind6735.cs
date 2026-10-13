using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind6735 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.PoliticsIncrease.Add(new KeyValuePair<int, int>(prob, increment));
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var prob = influence.GetIntParam();
        var increment = influence.GetIntParam2();

        person.PoliticsIncrease.Remove(new KeyValuePair<int, int>(prob, increment));
    }
}