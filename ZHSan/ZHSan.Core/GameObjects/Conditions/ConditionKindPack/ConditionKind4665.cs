using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4665 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.BubingExperience - markedPerson.BubingExperience < condition.GetIntParam();
    }
}