using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4675 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.QibingExperience - markedPerson.QibingExperience < condition.GetIntParam();
    }
}