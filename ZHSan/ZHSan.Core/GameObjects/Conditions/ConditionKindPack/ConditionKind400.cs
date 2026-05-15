using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind400 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.StratagemExperience >= condition.GetIntParam();
    }
}