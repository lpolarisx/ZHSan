using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2315 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.Meinvkongjian < condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return faction.meinvkongjian() < condition.GetIntParam();
    }
}