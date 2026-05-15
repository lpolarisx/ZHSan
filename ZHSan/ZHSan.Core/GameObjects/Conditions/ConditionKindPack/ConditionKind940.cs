using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind940 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && Person.GetIdealOffset(person, person.BelongedFactionWithPrincess.Leader) > condition.GetIntParam();

        return result;
    }
}