using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4810 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Loyalty - markedPerson.Loyalty >= condition.GetIntParam();
    }
}