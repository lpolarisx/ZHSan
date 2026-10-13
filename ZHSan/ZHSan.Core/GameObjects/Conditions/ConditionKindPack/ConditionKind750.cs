using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind750 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Tags.Contains(condition.Parameter + ",");
    }
}