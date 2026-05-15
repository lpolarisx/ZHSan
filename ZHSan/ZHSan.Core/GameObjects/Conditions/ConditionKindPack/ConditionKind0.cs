using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind0 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.InternalExperience >= condition.GetIntParam();
    }
}