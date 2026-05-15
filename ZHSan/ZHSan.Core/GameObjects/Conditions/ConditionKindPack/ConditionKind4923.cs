using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4923 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return !person.Brothers.GameObjects.Contains(markedPerson);
    }
}