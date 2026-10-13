using System.Runtime.Serialization;
namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect5 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        person.StrengthExperience += eventEffect.GetIntParam();
    }
}