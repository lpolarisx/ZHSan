using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind4 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.IncrementOfMoraleAbility += influence.GetIntParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.IncrementOfMoraleAbility -= influence.GetIntParam();
    }
}