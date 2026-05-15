using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind240 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.QixieExperience >= condition.GetIntParam();
    }
}