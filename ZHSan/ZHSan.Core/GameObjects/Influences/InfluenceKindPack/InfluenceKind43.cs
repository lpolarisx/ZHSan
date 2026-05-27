using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind43 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        var multiple = influence.GetIntParam() - 1;

        if (person.LocationTroop != null && multiple > 0)
        {
            person.LocationTroop.MultipleOfStratagemTechniquePoint += multiple;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        var multiple = influence.GetIntParam() - 1;

        if (person.LocationTroop != null && multiple > 0)
        {
            person.LocationTroop.MultipleOfStratagemTechniquePoint -= multiple;
        }
    }
}