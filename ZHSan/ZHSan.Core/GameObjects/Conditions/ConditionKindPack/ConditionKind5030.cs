using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind5030 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        return Check(condition);
    }

    public override bool CheckConditionKind(Condition condition, Architecture architecture)
    {
        return Check(condition);
    }

    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return Check(condition);
    }

    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        return Check(condition);
    }

    private bool Check(Condition condition)
    {
        return Session.Current.Scenario.Date.Day >= condition.GetIntParam();
    }
}