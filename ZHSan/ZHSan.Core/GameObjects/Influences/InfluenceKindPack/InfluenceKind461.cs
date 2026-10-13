using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind461 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.RateIncrementOfJailBreakAbility += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.RateIncrementOfJailBreakAbility -= influence.GetFloatParam();
    }
}