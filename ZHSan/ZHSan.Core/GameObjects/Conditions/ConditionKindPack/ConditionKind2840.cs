using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2840 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        if (arch.BelongedFaction == null) return false;

        var leader = arch.BelongedFaction.Leader;

        foreach (Person person in arch.Feiziliebiao)
        {
            if (person == leader)
            {
                return true;
            }
        }

        return false;
    }
}