using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind650 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var influenceId = condition.GetIntParam();

        foreach (Skill i in person.Skills.Skills.Values)
        {
            if (i.Influences.HasInfluence(influenceId)) return true;
        }
        foreach (Title t in person.Titles)
        {
            if (t.Influences.HasInfluence(influenceId)) return true;
        }
        foreach (Stunt i in person.Stunts.Stunts.Values)
        {
            if (i.Influences.HasInfluence(influenceId)) return true;
        }
        return false;
    }
}