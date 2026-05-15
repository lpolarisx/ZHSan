using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind890 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        var result = person.BelongedFactionWithPrincess != null && person.BelongedFactionWithPrincess.LeaderID == condition.GetIntParam();

        return result;
    }
}