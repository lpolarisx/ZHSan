using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind150 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.StrengthIncludingExperience >= condition.GetIntParam();
    }
}