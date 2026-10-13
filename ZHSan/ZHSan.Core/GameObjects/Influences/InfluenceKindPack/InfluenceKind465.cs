using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind465 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.RateIncrementOfSearch += influence.GetFloatParam();
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.RateIncrementOfSearch -= influence.GetFloatParam();
    }
}