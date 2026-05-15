using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4913 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Brothers.GameObjects.Contains(markedPerson);
    }
}