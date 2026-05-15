using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind120 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.IntelligenceExperience >= condition.GetIntParam();
    }
}