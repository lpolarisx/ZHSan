using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind901 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture architecture)
    {
        return true;
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return true;
    }

    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return true;
    }

    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return true;
    }
}