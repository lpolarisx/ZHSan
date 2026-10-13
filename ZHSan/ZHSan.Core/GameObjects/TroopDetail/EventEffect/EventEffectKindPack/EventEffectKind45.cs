using System;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind45 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        var troop = person.LocationTroop;

        if (troop != null)
        {
            var fund = troop.zijin + eventEffect.GetIntParam();

            troop.zijin = Math.Clamp(fund, 0, troop.Army.Kind.zijinshangxian);
        }
    }
}