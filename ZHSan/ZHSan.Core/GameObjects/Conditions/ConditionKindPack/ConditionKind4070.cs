using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4070 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.HeldCaptiveCount >= condition.GetIntParam();
    }
}