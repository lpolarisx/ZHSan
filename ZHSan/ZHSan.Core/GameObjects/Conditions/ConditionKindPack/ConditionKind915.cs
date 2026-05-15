using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind915 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = !(person.BelongedFactionWithPrincess != null && (person.Father == person.BelongedFactionWithPrincess.Leader || person.Mother == person.BelongedFactionWithPrincess.Leader));

        return result;
    }
}