using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2515 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.ArmyQuantity < condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return faction.Army < condition.GetIntParam();
    }
}