using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind4030 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.StuntCanAttackAfterRout = true;
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.StuntCanAttackAfterRout = false;
        }
    }
}