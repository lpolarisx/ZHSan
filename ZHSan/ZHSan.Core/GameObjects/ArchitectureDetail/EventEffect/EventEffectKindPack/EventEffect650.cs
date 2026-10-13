using System;
using System.Runtime.Serialization;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect650 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var tag = eventEffect.Parameter;

        int index = person.Tags.IndexOf(tag + ",");
        if (index >= 0)
        {
            person.Tags.Remove(index, tag.Length + 2);
        }
    }
}