using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2700 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.Kind.ID == condition.GetIntParam();
    }
}