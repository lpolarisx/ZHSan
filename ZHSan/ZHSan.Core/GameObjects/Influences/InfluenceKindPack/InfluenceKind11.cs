using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind11 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.RateIncrementOfCommerceAbility += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.RateIncrementOfCommerceAbility -= influence.GetFloatParam();
    }
}