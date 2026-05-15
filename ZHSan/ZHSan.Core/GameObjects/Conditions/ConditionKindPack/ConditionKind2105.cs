using System.Runtime.Serialization;

namespace GameObjects.Conditions.ConditionKindPack;

[DataContract]
public class ConditionKind2105 : ConditionKind
{
    public override bool CheckConditionKind(Condition condition, Architecture arch)
    {
        return arch.AgricultureCeiling > 0 && arch.Agriculture / arch.AgricultureCeiling >= condition.GetFloatParam();
    }
}