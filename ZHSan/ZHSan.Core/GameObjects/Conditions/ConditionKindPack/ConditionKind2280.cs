using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2280 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.Food >= condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return faction.Food >= condition.GetIntParam();
    }
}