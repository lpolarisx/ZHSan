using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind951 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && !person.Closes(person.BelongedFactionWithPrincess.Leader);

        return result;
    }
}