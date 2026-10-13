using System.Runtime.Serialization;

namespace GameObjects.TroopDetail.EventEffect.EventEffectKindPack;

[DataContract]
public class EventEffectKind35 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person)
    {
        if (person.LocationTroop != null)
        {
            int number = person.LocationTroop.Army.LoseInjuredTroop(eventEffect.GetFloatParam());
            /*if (number != 0)
            {
                person.LocationTroop.RefreshOffence();
                person.LocationTroop.RefreshDefence();
                person.LocationTroop.IncrementNumberList.AddNumber(number, CombatNumberKind.人数, person.LocationTroop.Position);
                person.LocationTroop.ShowNumber = true;
            }*/
        }
    }
}