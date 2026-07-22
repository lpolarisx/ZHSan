using GameManager;
using System.Runtime.Serialization;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect330 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var titleId = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllTitles.TryGetValue(titleId, out var title))
        {
            foreach (var t in person.RealTitles)
            {
                if (t.KindId == title.KindId)
                {
                    Influence.PurifyInfluenceList(title.Influences.Values, person, Applier.Title, titleId);
                    person.RealTitles.Remove(title);
                }
            }
        }
    }
}