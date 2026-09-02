using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect2200 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Faction faction, Event e)
    {
        if (faction == null) return;

        var increment = eventEffect.GetIntParam();

        var diplomaticRelations = Session.Current.Scenario.GetDiplomaticRelationListByFactionID(faction.ID);

        foreach (var relation in diplomaticRelations)
        {
            relation.Relation += increment;
        }
    }
}