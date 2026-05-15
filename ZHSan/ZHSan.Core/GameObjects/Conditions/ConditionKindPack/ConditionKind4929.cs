using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4929 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return (person != markedPerson.Father && person != markedPerson.Mother) || person.Father == null || person.Mother == null;
    }
}