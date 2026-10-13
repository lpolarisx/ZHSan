using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind60 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        var troop = person.LocationTroop;

        if (troop != null)
        {   
            var rate = Session.Current.Scenario.GetTerrainDetailByPositionNoCheck(troop.Position).FireDamageRate;

            troop.SetOnFire(Session.Parameters.FireDamageScale * rate);
        }
    }
}