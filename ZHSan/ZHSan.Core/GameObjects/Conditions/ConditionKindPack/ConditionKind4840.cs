using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4840 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return Person.GetIdealOffset(person, markedPerson) >= condition.GetIntParam();
    }
}