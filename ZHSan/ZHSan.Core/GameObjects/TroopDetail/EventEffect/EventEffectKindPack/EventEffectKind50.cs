using System;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind50 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        var troop = person.LocationTroop;

        if (troop != null)
        {
            var rate = eventEffect.GetFloatParam() - 1;
            var quantity = (int)Math.Abs(troop.Quantity * rate);

            if (rate >= 0)
            {
                person.LocationTroop.IncreaseQuantity(quantity);
            }
            else
            {
                person.LocationTroop.DecreaseQuantity(quantity);
            }
        }
    }
}