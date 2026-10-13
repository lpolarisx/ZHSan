using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind1900 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        var conditionId = condition.GetIntParam();

        if (Session.Current.Scenario.GameCommonData.AllConditions.TryGetValue(conditionId, out var matchCondition))
        {
            return matchCondition.CheckCondition(troop.Leader);
        }

        return false;
    }
}