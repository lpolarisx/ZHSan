using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind24 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var multiple = influence.GetIntParam() - 1;

        if (multiple > 0)
        {
            person.MultipleOfMoraleReputation += multiple;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var multiple = influence.GetIntParam() - 1;

        if (multiple > 0)
        {
            person.MultipleOfMoraleReputation -= multiple;
        }
    }
}