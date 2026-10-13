using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind5030 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.StuntDayDecrementOfAttack += influence.GetIntParam();
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.StuntDayDecrementOfAttack -= influence.GetIntParam();
        }
    }
}