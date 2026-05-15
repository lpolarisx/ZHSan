using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind3215 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        var archId = condition.GetIntParam();

        foreach (Architecture arch in faction.Architectures)
        {
            if (arch.ID == archId)
            {
                return false;
            }
        }

        return true;
    }
}