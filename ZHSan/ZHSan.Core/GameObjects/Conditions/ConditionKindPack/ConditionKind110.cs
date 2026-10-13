using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind110 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.CommandExperience >= condition.GetIntParam();
    }
}