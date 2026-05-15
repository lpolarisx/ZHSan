using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind850 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && person.GetRelation(person.BelongedFactionWithPrincess.Leader) >= condition.GetIntParam();

        return result;
    }
}