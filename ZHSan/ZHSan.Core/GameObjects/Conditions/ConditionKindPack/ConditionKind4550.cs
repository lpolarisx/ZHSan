using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4550 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Glamour - markedPerson.Glamour >= condition.GetIntParam();
    }
}