using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind100 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.StrengthExperience >= condition.GetIntParam();
    }
}