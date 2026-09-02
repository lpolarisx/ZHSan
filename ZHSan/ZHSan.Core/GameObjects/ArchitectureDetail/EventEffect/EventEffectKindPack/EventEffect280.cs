using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect280 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var factionId = eventEffect.GetIntParam();
        var mergeFaction = Session.Current.Scenario.Factions.GetValueOrDefault(factionId);

        if (mergeFaction == null) return;

        var oldFaction = person.BelongedFaction;
        if (oldFaction != null && person == oldFaction.Leader)
        {
            oldFaction.ChangeFaction(mergeFaction);
            //oldFaction.Leader.InitialLoyalty();
        }
    }
}