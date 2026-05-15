using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind900 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Person person)
    {
        return false;
    }
}