using System.Runtime.Serialization;
using GameObjects.PersonDetail;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind650 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var influenceId = condition.GetIntParam();

        foreach (var skill in person.Skills.Values)
        {
            if (skill.Influences.ContainsKey(influenceId)) return true;
        }

        foreach (Title title in person.Titles)
        {
            if (title.Influences.ContainsKey(influenceId)) return true;
        }

        foreach (var stunt in person.Stunts.Values)
        {
            if (stunt.Influences.ContainsKey(influenceId)) return true;
        }

        return false;
    }
}