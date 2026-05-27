using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind300 : InfluenceKind
{
    public override bool IsVaild(Influence influence, Person person)
    {
        var troop = person.LocationTroop;

        return troop != null && troop.Army != null && troop.Army.KindID == influence.GetIntParam();
    }
}