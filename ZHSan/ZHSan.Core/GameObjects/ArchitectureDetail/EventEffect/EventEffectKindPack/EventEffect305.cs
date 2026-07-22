using GameManager;
using System.Runtime.Serialization;
using GameObjects.Influences;

namespace GameObjects.ArchitectureDetail.EventEffect;

[DataContract]
public class EventEffect305 : EventEffectKind
{
    public override void ApplyEffectKind(EventEffect eventEffect, Person person, Event e)
    {
        var skillId = eventEffect.GetIntParam();
        if (Session.Current.Scenario.GameCommonData.AllSkills.TryGetValue(skillId, out var skill))
        {
            person.Skills.Remove(skillId);
            Influence.PurifyInfluenceList(skill.Influences.Values, person, Applier.Skill, skillId);
        }
    }
}