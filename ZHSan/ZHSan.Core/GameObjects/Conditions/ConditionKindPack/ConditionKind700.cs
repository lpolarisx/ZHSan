using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind700 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return GameObject.Chance(condition.GetIntParam());
    }

    public override bool CheckConditionKind(Condition condition, Architecture architecture)
    {
        return GameObject.Chance(condition.GetIntParam());
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return GameObject.Chance(condition.GetIntParam());
    }

    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return GameObject.Chance(condition.GetIntParam());
    }
}