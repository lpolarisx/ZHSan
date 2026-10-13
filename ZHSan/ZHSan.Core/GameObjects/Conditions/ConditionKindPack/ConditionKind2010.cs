using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2010 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return CheckConditionKind(condition, arch.BelongedFaction);
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        var count = condition.GetIntParam();
        var val = condition.GetIntParam2();

        int i = 0;
        foreach (Person p in faction.Persons)
        {
            if (p.Command >= val)
            {
                i++;
                if (i >= count)
                    return true;
            }
        }
        
        return false;
    }
}