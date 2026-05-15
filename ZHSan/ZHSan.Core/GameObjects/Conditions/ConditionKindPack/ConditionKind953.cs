using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind953 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && !person.Hates(person.BelongedFactionWithPrincess.Leader);

        return result;
    }
}