using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4530 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Intelligence - markedPerson.Intelligence >= condition.GetIntParam();
    }
}