using GameManager;
using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind505 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        Treasure t = Session.Current.Scenario.Treasures.GetGameObject(condition.GetIntParam()) as Treasure;
        return !person.Treasures.GameObjects.Contains(t);
    }

    public override bool CheckConditionKind(Condition condition, Architecture architecture)
    {
        foreach (Person p in architecture.Persons)
        {
            if (CheckConditionKind(condition, p))
            {
                return true;
            }
        }
        return false;
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        foreach (Person p in faction.Persons)
        {
            if (CheckConditionKind(condition, p))
            {
                return true;
            }
        }
        return false;
    }

    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        foreach (Person p in troop.Persons)
        {
            if (CheckConditionKind(condition, p))
            {
                return true;
            }
        }
        return false;
    }
}