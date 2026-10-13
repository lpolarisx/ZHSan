using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4900 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.GetRelation(markedPerson) >= condition.GetIntParam();
    }
}