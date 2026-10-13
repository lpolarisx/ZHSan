using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2820 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        var personId = condition.GetIntParam();

        foreach (Captive captive in arch.Captives)
        {
            if (captive.CaptivePersonID == personId)
            {
                return true;
            }
        }

        return false;
    }
}