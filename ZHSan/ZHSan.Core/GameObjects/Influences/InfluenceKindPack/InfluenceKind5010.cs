using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind5010 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.MoraleNoChanceAfterAttacked = true;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.MoraleNoChanceAfterAttacked = false;
        }
    }
}