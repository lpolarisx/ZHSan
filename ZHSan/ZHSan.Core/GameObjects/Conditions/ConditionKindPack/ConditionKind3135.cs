using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind3135 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        var regoinId = condition.GetIntParam();
        var requiredCount = condition.GetIntParam2();

        int c = 0;
        foreach (Architecture a in faction.Architectures)
        {
            if (a.LocationState.LinkedRegion.ID == regoinId)
            {
                c++;
            }
        }

        return c < requiredCount;
    }
}