using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect500 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        int treasureId = eventEffect.GetIntParam();
        var treasure = Session.Current.Scenario.Treasures.GetValueOrDefault(treasureId);

        if (treasure == null) return;

        if (treasure.BelongedPerson != null)
        {
            treasure.BelongedPerson.LoseTreasure(treasure);
        }

        person.ReceiveTreasure(treasure);
    }
}