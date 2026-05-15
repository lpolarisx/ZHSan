using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4715 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.StratagemExperience - markedPerson.StratagemExperience < condition.GetIntParam();
    }
}