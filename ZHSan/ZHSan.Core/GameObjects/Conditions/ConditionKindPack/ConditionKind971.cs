using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind971 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return !person.Sex;
    }
}