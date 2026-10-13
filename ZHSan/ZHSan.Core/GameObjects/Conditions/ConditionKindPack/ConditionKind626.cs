using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind626 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var skillId = condition.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllSkills.TryGetValue(skillId, out var skill))
        {
            return !skill.CanLearn(person);
        }

        return true;
    }
}