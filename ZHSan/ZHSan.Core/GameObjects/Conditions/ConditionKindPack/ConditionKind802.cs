using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind802 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.BelongedFaction != null && person.BelongedFaction.Leader == person;
    }
}