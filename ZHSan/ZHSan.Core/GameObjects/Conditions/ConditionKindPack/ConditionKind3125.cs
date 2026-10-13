using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind3125 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        var stateId = condition.GetIntParam();
        var requiredCount = condition.GetIntParam2();

        int c = 0;
        foreach (Architecture a in faction.Architectures)
        {
            if (a.LocationState.ID == stateId)
            {
                c++;
            }
        }

        return c < requiredCount;
    }
}