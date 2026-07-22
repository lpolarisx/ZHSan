using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind646 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var stuntId = condition.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllStunts.TryGetValue(stuntId, out var stunt))
        {
            return !stunt.IsLearnable(person);
        }

        return true;
    }
}