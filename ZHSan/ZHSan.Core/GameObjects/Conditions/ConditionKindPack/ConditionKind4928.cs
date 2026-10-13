using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4928 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person != markedPerson.Father && person != markedPerson.Mother;
    }
}