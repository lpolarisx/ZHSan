using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind916 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && (person.BelongedFactionWithPrincess.Leader.Father == person || person.BelongedFactionWithPrincess.Leader.Mother == person);

        return result;
    }
}