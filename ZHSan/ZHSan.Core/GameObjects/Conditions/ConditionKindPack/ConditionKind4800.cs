using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4800 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Age - markedPerson.Age >= condition.GetIntParam();
    }
}