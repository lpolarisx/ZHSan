using GameManager;
using System.Runtime.Serialization;
using GameObjects.FactionDetail;
using System.Collections.Generic;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect2300 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Faction faction, Event e)
    {
        var increment = eventEffect.GetIntParam();

        var factionId = faction.ID;
        var scenario = Session.Current.Scenario;

        var otherFactionId = eventEffect.GetIntParam2();
        var otherFaction = scenario.Factions.GetValueOrDefault(otherFactionId);

        if (otherFaction == null) return;

        var diplomaticRelations = scenario.GetDiplomaticRelationListByFactionID(factionId);
        foreach (var relation in diplomaticRelations)
        {
            if ((relation.RelationFaction1ID == factionId && relation.RelationFaction2ID == otherFactionId) || (relation.RelationFaction1ID == otherFactionId && relation.RelationFaction2ID == factionId))
            {
                scenario.ChangeDiplomaticRelation(factionId, otherFactionId, increment);
            }
        }
    }
}