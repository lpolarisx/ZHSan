using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind290 : InfluenceKind
{
    public override bool IsVaild(Influence influence, Person person)
    {
        var troop = person.LocationTroop;

        //兼容旧存档？
        return troop != null && troop.Army != null && troop.Army.Kind != null && (int)troop.Army.Kind.Type == influence.GetIntParam();
    }
}