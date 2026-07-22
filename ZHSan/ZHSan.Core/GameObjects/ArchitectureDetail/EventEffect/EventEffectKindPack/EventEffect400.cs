using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect400 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var id = eventEffect.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllCharacterKinds.TryGetValue(id, out var characterKind))
        {
            person.Character = characterKind;
        }
    }
}