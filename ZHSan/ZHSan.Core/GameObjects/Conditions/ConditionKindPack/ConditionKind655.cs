using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind655 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var influenceId = condition.GetIntParam();

        foreach (var skill in person.Skills.Values)
        {
            if (skill.Influences.ContainsKey(influenceId)) return false;
        }
        foreach (var title in person.Titles)
        {
            if (title.Influences.ContainsKey(influenceId)) return false;
        }
        foreach (var stunt in person.Stunts.Values)
        {
            if (stunt.Influences.ContainsKey(influenceId)) return false;
        }
        return true;
    }
}