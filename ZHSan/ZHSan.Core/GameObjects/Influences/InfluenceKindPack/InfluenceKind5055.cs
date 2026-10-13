using System.Runtime.Serialization;

namespace GameObjects.Influences.InfluenceKindPack;

[DataContract]
public class InfluenceKind5055 : InfluenceKind
{
    public override void ApplyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.ChanceOfTrippleDamage += influence.GetIntParam();
        }
    }

    public override void PurifyInfluenceKind(Influence influence, Person person)
    {
        if (person.LocationTroop != null)
        {
            person.LocationTroop.ChanceOfTrippleDamage -= influence.GetIntParam();
        }
    }
}