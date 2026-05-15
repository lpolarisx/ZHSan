using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind958 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && person.Brothers.GameObjects.Contains(person.BelongedFactionWithPrincess.Leader);

        return result;
    }
}