using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect310 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var titleId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllTitles.TryGetValue(titleId, out var title))
        {
            person.LearnTitle(title);
        }
    }
}