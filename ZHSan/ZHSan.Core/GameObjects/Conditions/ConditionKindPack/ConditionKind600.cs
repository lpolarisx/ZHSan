using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind600 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Character.ID == condition.GetIntParam();
    }
}