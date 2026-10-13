using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4580 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Karma - markedPerson.Karma >= condition.GetIntParam();
    }
}