using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4545 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Politics - markedPerson.Politics < condition.GetIntParam();
    }
}