using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind315 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OfficerMerit < condition.GetIntParam();
    }
}