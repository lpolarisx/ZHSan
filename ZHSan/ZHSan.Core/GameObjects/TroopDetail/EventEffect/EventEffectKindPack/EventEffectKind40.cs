using System;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind40 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        var troop = person.LocationTroop;

        if (troop != null)
        {
            var food = troop.Food + eventEffect.GetIntParam();

            troop.Food = Math.Clamp(food, 0, troop.FoodMax);
        }
    }
}