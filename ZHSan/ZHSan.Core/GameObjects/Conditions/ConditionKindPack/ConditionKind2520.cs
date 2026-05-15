using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2520 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.ArmyScale >= condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return faction.ArmyScale >= condition.GetIntParam();
    }
}