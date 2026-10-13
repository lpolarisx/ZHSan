using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind300 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Reputation >= condition.GetIntParam();
    }
}