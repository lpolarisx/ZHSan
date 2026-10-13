using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind370 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.IntelligenceIncludingExperience < condition.GetIntParam();
    }
}