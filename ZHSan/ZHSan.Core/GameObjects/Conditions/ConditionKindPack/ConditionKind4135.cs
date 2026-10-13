using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind4135 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return person.Tiredness < condition.GetIntParam();
    }
}