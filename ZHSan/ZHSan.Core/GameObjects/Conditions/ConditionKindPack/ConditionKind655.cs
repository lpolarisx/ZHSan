using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind655 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var influenceId = condition.GetIntParam();

        foreach (Skill i in person.Skills.Skills.Values)
        {
            if (i.Influences.HasInfluence(influenceId)) return false;
        }
        foreach (Title t in person.Titles)
        {
            if (t.Influences.HasInfluence(influenceId)) return false;
        }
        foreach (Stunt i in person.Stunts.Stunts.Values)
        {
            if (i.Influences.HasInfluence(influenceId)) return false;
        }
        return true;
    }
}