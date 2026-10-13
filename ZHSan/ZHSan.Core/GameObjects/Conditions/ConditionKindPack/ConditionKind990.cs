using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind990 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.GivenName == condition.Parameter;
    }
}