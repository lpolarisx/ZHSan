using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4705 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.QixieExperience - markedPerson.QixieExperience < condition.GetIntParam();
    }
}