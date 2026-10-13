using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind545 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = !(person.BelongedFactionWithPrincess != null && person.LocationArchitecture == person.BelongedFactionWithPrincess.Leader.LocationArchitecture);

        return result;
    }
}