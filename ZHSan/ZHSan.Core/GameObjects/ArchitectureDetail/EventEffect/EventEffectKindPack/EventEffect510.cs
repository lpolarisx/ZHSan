using GameGlobal;
using GameManager;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect510 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        int treasureId = eventEffect.GetIntParam();
        var treasure = Session.Current.Scenario.Treasures.GetValueOrDefault(treasureId);

        if (treasure == null) return;

        if (treasure.BelongedPerson != null && treasure.BelongedPerson == person)
        {
            person.LoseTreasure(treasure);
            treasure.Available = false;
            treasure.HidePlace = StaticMethods.GetRandomItem(Session.Current.Scenario.Architectures.Values.ToList());
        }
    }
}