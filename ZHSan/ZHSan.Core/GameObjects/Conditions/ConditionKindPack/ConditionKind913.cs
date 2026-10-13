using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind913 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess == null || !person.BelongedFactionWithPrincess.Leader.HasCloseStrainTo(person);

        return result;
    }
}