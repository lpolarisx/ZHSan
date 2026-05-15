using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2905 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.ID != condition.GetIntParam();
    }
}