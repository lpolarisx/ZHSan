using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4570 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.OfficerMerit - markedPerson.OfficerMerit >= condition.GetIntParam();
    }
}