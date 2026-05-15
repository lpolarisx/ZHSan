using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind880 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.ID == condition.GetIntParam();
    }
}