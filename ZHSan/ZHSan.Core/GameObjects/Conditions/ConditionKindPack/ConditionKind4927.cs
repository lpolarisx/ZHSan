using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4927 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Mother != markedPerson;
    }
}