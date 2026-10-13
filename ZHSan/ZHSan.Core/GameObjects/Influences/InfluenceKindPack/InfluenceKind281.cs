using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind281 : InfluenceKind
{
    public InfluenceKind281()
    {
        TroopLeaderValid = true;
    }

    public override bool IsVaild(Influence influence, Person person)
    {
        return person.LocationTroop != null && person.LocationTroop.Leader == person;
    }
}