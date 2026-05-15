using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind515 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.TreasureCount < condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Architecture architecture)
    {
        int result = 0;
        foreach (Person p in architecture.Persons)
        {
            result += p.TreasureCount;
        }
        return result < condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Faction faction)
    {
        int result = 0;
        foreach (Person p in faction.Persons)
        {
            result += p.TreasureCount;
        }
        return result < condition.GetIntParam();
    }

    public override bool CheckConditionKind(Condition condition, Troop troop)
    {
        int result = 0;
        foreach (Person p in troop.Persons)
        {
            result += p.TreasureCount;
        }
        return result < condition.GetIntParam();
    }
}