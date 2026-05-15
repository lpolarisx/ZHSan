using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind641 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return Session.Current.Scenario.GameCommonData.AllStunts.GetStunt(condition.GetIntParam()).IsLearnable(person);
    }
}