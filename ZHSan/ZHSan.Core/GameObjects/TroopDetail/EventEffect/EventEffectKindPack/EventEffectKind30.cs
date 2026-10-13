using GameObjects.Animations;
using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind30 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        var troop = person.LocationTroop;

        if (troop != null)
        {
            int number = troop.Army.Recovery(eventEffect.GetFloatParam());
            if (number != 0)
            {
                troop.RefreshOffence();
                troop.RefreshDefence();
                troop.IncrementNumberList.AddNumber(number, CombatNumberKind.人数, troop.Position);
                troop.ShowNumber = true;
            }
        }
    }
}