using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
class ConditionKind2910 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var conditionId = condition.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllConditions.TryGetValue(conditionId, out var matchCondition) && arch.Mayor != null)
        {
            return matchCondition.CheckCondition(arch.Mayor);
        }

        return false;
    }
}