using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind471 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        person.InevitableSuccessOfJailBreak = true;
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        person.InevitableSuccessOfJailBreak = false;
    }
}