using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind626 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return !Session.Current.Scenario.GameCommonData.AllSkills.GetSkill(condition.GetIntParam()).CanLearn(person);
    }
}