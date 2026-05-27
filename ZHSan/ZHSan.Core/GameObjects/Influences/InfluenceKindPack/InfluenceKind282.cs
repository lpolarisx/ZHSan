using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind282 : InfluenceKind
{
    public override bool IsVaild(Influence influence, Person person)
    {
        return person.BelongedArchitecture != null && person.BelongedArchitecture.Mayor == person;
    }
}