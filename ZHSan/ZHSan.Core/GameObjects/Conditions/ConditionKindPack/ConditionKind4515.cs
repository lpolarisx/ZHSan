using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4515 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Strength - markedPerson.Strength < condition.GetIntParam();
    }
}