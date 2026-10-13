using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind475 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.InevitableSuccessOfSearch = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.InevitableSuccessOfSearch = false;
    }
}