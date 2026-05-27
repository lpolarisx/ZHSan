using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind30 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.InternalNoFundNeeded = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.InternalNoFundNeeded = false;
    }
}