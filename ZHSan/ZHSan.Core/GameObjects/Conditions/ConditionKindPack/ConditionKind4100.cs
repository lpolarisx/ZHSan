using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4100 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.StratagemFailCount >= condition.GetIntParam();
    }
}