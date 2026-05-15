using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind325 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Karma < condition.GetIntParam();
    }
}