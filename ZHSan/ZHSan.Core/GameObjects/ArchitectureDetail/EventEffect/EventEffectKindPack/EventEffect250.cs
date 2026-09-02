using GameManager;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect250 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        int factionId = eventEffect.GetIntParam();
        var targetFaction = Session.Current.Scenario.Factions.GetValueOrDefault(factionId);
        /*
        if (targetFaction != null)
        {
            throw new Exception("targetFaction 为" + targetFaction.Name);
        }
        */

        var location = person.LocationArchitecture;

        if (location != null)
        {
            if (person.BelongedFaction == null && targetFaction != null)
            {
                //person.Status = GameObjects.PersonDetail.PersonStatus.Normal;
                person.MoveToArchitecture(targetFaction.Capital, null, true, true, person.BelongedFaction);
                person.ChangeFaction(targetFaction);
            }
            else if (location.BelongedFaction != null)
            {
                person.MoveToArchitecture(targetFaction.Capital, null, true, true, person.BelongedFaction);
                person.ChangeFaction(targetFaction);
            }
        }
    }
}