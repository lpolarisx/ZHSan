using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4685 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.NubingExperience - markedPerson.NubingExperience < condition.GetIntParam();
    }
}