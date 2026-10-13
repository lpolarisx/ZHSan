using GameManager;
using System.Runtime.Serialization;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect315 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var titleId = eventEffect.GetIntParam();
        
        if (Session.Current.Scenario.GameCommonData.AllTitles.TryGetValue(titleId, out var title) && person.RealTitles.Contains(title))
        {
            Influence.PurifyInfluenceList(title.Influences.Values, person, Applier.Title, titleId);
            person.RealTitles.Remove(title);
        }
    }
}