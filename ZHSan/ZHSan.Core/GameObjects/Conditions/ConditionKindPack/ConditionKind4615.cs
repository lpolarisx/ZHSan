using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4615 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.StrengthExperience - markedPerson.StrengthExperience < condition.GetIntParam();
    }
}