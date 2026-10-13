using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind2 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.IncrementOfTechnologyAbility += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.IncrementOfTechnologyAbility -= influence.GetIntParam();
    }
}